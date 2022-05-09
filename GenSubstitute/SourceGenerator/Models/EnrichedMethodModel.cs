using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.Models
{
    /// <summary>
    /// Contains data derived from <see cref="MethodModel"/>,
    /// which is not required for caching, but is useful in source generation. 
    /// </summary>
    internal readonly struct EnrichedMethodModel
    {
        // E.g. <T1, T2>
        public readonly string GenericNames;
        
        // E.g. $"SomeMethod<{typeof(T1).FullName}, {typeof(T2).FullName}>
        public readonly string ResolvedMethodName;
        
        // E.g. "Arg<int>? intArg, Arg<double>? doubleArg"
        public readonly string ArgParameters;

        // E.g. "intArg ?? Arg<int>.Default, doubleArg ?? Arg<double>.Default"
        public readonly string SafeParameterNames;

        // E.g. ConfiguredFunc<int, double>
        public readonly string ConfiguredCallType;
        
        // E.g. ReceivedCall<int>
        public readonly string ReceivedCallType;
        
        public EnrichedMethodModel(MethodModel method)
        {
            if (method.GenericParameterNames.Length == 0)
            {
                GenericNames = "";
                ResolvedMethodName = $"\"{method.Name}\"";
            }
            else
            {
                GenericNames = $"<{BuildList(method.GenericParameterNames)}>";
                    
                var fullNames = method.GenericParameterNames
                    .Select(p => $"{{typeof({p}).FullName}}");
                ResolvedMethodName = $"$\"{method.Name}<{BuildList(fullNames)}>\"";
            }
            
            ArgParameters = BuildList(method.Parameters
                .Select(p =>
                {
                    var argType = p.RefKind switch
                    {
                        RefKind.Ref => $"RefArg<{p.Type}>",
                        // TODO handle other types
                        _ => $"Arg<{p.Type}>",
                    };
                    
                    return p.HasDefaultValue
                        ? $"{argType}? {p.Name} = default"
                        : $"{argType}? {p.Name}";
                }));
                
            SafeParameterNames = BuildList(method.Parameters
                .Select(p =>
                {
                    // TODO, cache these?
                    var argType = p.RefKind switch
                    {
                        RefKind.Ref => $"RefArg<{p.Type}>",
                        // TODO handle other types
                        _ => $"Arg<{p.Type}>",
                    };
                    
                    var fallbackValue = p.HasDefaultValue && p.DefaultValue is { } defaultValue
                        ? $"new {argType}({DefaultValueToString(defaultValue, p.Type)})"
                        : $"{argType}.Default";
                    return $"{p.Name} ?? {fallbackValue}";
                }));
            
            if (method.Parameters.Length == 0 && method.ReturnsVoid)
            {
                ConfiguredCallType = nameof(ConfiguredAction);
            }
            else
            {
                var callType = method.ReturnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
                var parameterArguments = method.Parameters
                    .Select(p => p.RefKind switch
                    {
                        RefKind.Ref => $"RefArg<{p.Type}>",
                        // TODO handle other types
                        _ => p.Type,
                    });
                
                var allArguments = method.ReturnsVoid
                    ? parameterArguments
                    : parameterArguments.Append(method.ReturnType);

                ConfiguredCallType = $"{callType}<{BuildList(allArguments)}>";
            }
            
            // TODO TODO TODO, ugly
            var types = method.Parameters
                .Select(p => p.RefKind switch
                {
                    RefKind.Ref => $"RefArg<{p.Type}>",
                    // TODO handle other types
                    _ => p.Type,
                });
            
            ReceivedCallType = method.Parameters.Length == 0
                ? "ReceivedCall"
                : $"ReceivedCall<{BuildList(types)}>";
        }

        // TODO, are there cases where this wouldn't work?
        private static string DefaultValueToString(object obj, string typeName) =>
            $"({typeName}){SymbolDisplay.FormatPrimitive(obj, quoteStrings: true, useHexadecimalNumbers: false)}";
    }
}
