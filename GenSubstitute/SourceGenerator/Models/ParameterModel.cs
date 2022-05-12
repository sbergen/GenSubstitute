using System;
using System.Collections.Generic;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct ParameterModel : IEquatable<ParameterModel>
    {
        public readonly string Type;
        public readonly string Name;
        public readonly bool HasDefaultValue;
        public readonly object? DefaultValue;
        public readonly RefKind RefKind;
        
        public ParameterModel(IParameterSymbol symbol)
        {
            // This already contains nullability (don't know why)
            Type = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            Name = symbol.Name;
            HasDefaultValue = symbol.HasExplicitDefaultValue;
            DefaultValue = HasDefaultValue ? symbol.ExplicitDefaultValue : null;
            RefKind = symbol.RefKind;
        }

        public bool Equals(ParameterModel other) =>
            Type == other.Type &&
            Name == other.Name &&
            HasDefaultValue == other.HasDefaultValue &&
            EqualityComparer<object?>.Default.Equals(DefaultValue, other.DefaultValue);
    }
}
