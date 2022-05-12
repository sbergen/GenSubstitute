using System;
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
        public readonly string Type;

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
        
        // E.g. "", "ref ", or " out"
        public readonly string RefKindString;

        public readonly bool IsRef;
        public readonly bool IsOut;
        
        public EnrichedParameterModel(ParameterModel parameter)
        {
            Name = parameter.Name;
            Type = parameter.Type;
            
            // TODO, is there a better way to do this?
            RefKindString = parameter.RefKind switch
            {
                RefKind.None => "",
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            WrappedType = parameter.RefKind switch
            {
                RefKind.Ref => $"RefArg<{parameter.Type}>",
                RefKind.Out => $"OutArg<{parameter.Type}>",
                _ => $"Arg<{parameter.Type}>",
            };

            IsRef = parameter.RefKind is RefKind.Ref;
            IsOut = parameter.RefKind is RefKind.Out;

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
