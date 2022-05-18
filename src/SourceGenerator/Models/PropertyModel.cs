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
        public readonly string HelperSubType;
        
        public PropertyModel(IPropertySymbol property)
        {
            Name = property.Name;
            Type = property.Type.FullyQualifiedTypeNameWithNullability(property.NullableAnnotation);

            GetMethodName = property.GetMethod?.Name;
            SetMethodName = property.SetMethod?.Name;

            HelperSubType = (property.IsReadOnly, property.IsWriteOnly) switch
            {
                (true, _) => "ReadOnly",
                (_, true) => "WriteOnly",
                _ => "ReadWrite",
            };
        }

        // Get and set method names should be deterministic
        public bool Equals(PropertyModel other) =>
            Name == other.Name &&
            Type == other.Type;
    }
}
