using System;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeLookupInfo : IEquatable<TypeLookupInfo>
    {
        public readonly string MetadataName;
        public readonly string FullyQualifiedName;

        public TypeLookupInfo(INamedTypeSymbol symbol)
        {
            var nameSource = symbol.IsGenericType
                ? symbol.ConstructedFrom
                : symbol;

            MetadataName = nameSource.FullyQualifiedMetadataName();
            FullyQualifiedName = nameSource.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public bool Equals(TypeLookupInfo other) => MetadataName == other.MetadataName;
    }
}
