using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct ParameterModel : IEquatable<ParameterModel>
    {
        public readonly string Type;
        public readonly string Name;
        public readonly bool HasDefaultValue;
        public readonly object? DefaultValue;
        
        public ParameterModel(IParameterSymbol symbol)
        {
            Type = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                   + (symbol.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "");
            Name = symbol.Name;
            HasDefaultValue = symbol.HasExplicitDefaultValue;
            DefaultValue = HasDefaultValue ? symbol.ExplicitDefaultValue : null;
        }

        public bool Equals(ParameterModel other) =>
            Type == other.Type &&
            Name == other.Name &&
            HasDefaultValue == other.HasDefaultValue &&
            EqualityComparer<object?>.Default.Equals(DefaultValue, other.DefaultValue);
    }
}
