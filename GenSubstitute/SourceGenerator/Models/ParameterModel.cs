using System;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct ParameterModel : IEquatable<ParameterModel>
    {
        public readonly string Type;
        public readonly string Name;
        
        public ParameterModel(IParameterSymbol symbol)
        {
            Type = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                   + (symbol.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "");
            Name = symbol.Name;
        }

        public bool Equals(ParameterModel other) =>
            Type == other.Type && Name == other.Name;
    }
}
