using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.SourceGenerator.Models
{
    /// <summary>
    /// Contains data derived from <see cref="ParameterModel"/>,
    /// which is not required for caching, but is useful in source generation. 
    /// </summary>
    internal readonly struct EnrichedParameterModel
    {
        public readonly string Name;
        
        // e.g. Arg<int> or RefArg<int>
        public readonly string WrappedType;

        // Type to be used with e.g. ReceivedCall or ConfiguredFunc
        // e.g. int or RefArg<int>
        public readonly string CallObjectType;
        
        // e.g. " = default" or ""
        // Note that the actual value will be assigned separately
        public readonly string DefaultValueDeclaration;

        // E.g. new Arg<int>(42) or Arg<int>.Default
        public readonly string DefaultValueConstructor;
        
        public EnrichedParameterModel(ParameterModel parameter)
        {
            Name = parameter.Name;
            
            // TODO handle other types
            WrappedType = parameter.RefKind switch
            {
                RefKind.Ref => $"RefArg<{parameter.Type}>",
                _ => $"Arg<{parameter.Type}>",
            };

            CallObjectType = parameter.RefKind == RefKind.None
                ? parameter.Type
                : WrappedType;

            DefaultValueDeclaration = parameter.HasDefaultValue
                ? " = default"
                : "";
            
            DefaultValueConstructor = parameter.HasDefaultValue && parameter.DefaultValue is { } defaultValue
                ? $"new {WrappedType}({DefaultValueToString(defaultValue, parameter.Type)})"
                : $"{WrappedType}.Default";
        }
        
        // TODO, are there cases where this wouldn't work?
        private static string DefaultValueToString(object obj, string typeName) =>
            $"({typeName}){SymbolDisplay.FormatPrimitive(obj, quoteStrings: true, useHexadecimalNumbers: false)}";
    }
}
