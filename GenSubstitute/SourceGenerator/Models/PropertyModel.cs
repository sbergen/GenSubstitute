using System;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct PropertyModel : IEquatable<PropertyModel>
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string? GetMethodName;
        public readonly string? SetMethodName;
        
        public PropertyModel(IPropertySymbol property)
        {
            Name = property.Name;
            Type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            GetMethodName = property.GetMethod is { } getMethod
                ? InternalName.Make(getMethod.Name)
                : null;
            
            SetMethodName = property.SetMethod is { } setMethod
                ? InternalName.Make(setMethod.Name)
                : null;
        }

        public bool Equals(PropertyModel other)
        {
            throw new NotImplementedException();
        }
    }
}
