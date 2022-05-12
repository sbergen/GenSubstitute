using System;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct PropertyModel : IEquatable<PropertyModel>
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string BackingFieldName;
        public readonly bool HasGet;
        public readonly bool HasSet;
        
        public PropertyModel(IPropertySymbol property)
        {
            Name = property.Name;
            Type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            HasGet = !property.IsWriteOnly;
            HasSet = !property.IsReadOnly;
            BackingFieldName = InternalName.Make(property.Name, "BackingField");
        }

        public bool Equals(PropertyModel other)
        {
            throw new NotImplementedException();
        }
    }
}
