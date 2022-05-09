using System.Linq;
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
                .Select(p => p.HasDefaultValue
                    ? $"Arg<{p.Type}>? {p.Name} = default"
                    : $"Arg<{p.Type}>? {p.Name}"));
                
            SafeParameterNames = BuildList(method.Parameters
                .Select(p =>
                {
                    var fallbackValue = p.HasDefaultValue && p.DefaultValue is { } defaultValue
                        ? $"new Arg<{p.Type}>({DefaultValueToString(defaultValue, p.Type)})"
                        : $"Arg<{p.Type}>.Default";
                    return $"{p.Name} ?? {fallbackValue}";
                }));
            
            if (method.Parameters.Length == 0 && method.ReturnsVoid)
            {
                ConfiguredCallType = nameof(ConfiguredAction);
            }
            else
            {
                var callType = method.ReturnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
                var parameterArguments = method.Parameters.Select(p => p.Type);
                var allArguments = method.ReturnsVoid
                    ? parameterArguments
                    : parameterArguments.Append(method.ReturnType);

                ConfiguredCallType = $"{callType}<{BuildList(allArguments)}>";
            }
            
            ReceivedCallType = method.Parameters.Length == 0
                ? "ReceivedCall"
                : $"ReceivedCall<{BuildList(method.Parameters.Select(p => p.Type))}>";
        }
        
        // TODO, are there cases where this wouldn't work?
        private static string DefaultValueToString(object obj, string typeName) =>
            $"({typeName}){SymbolDisplay.FormatPrimitive(obj, quoteStrings: true, useHexadecimalNumbers: false)}";
    }
}
