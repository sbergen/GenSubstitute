using System;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeLookupInfo : IEquatable<TypeLookupInfo>
    {
        public readonly string FullyQualifiedName;
        public readonly string Name;

        public TypeLookupInfo(INamedTypeSymbol symbol)
        {
            var nameSource = symbol.IsGenericType
                ? symbol.ConstructedFrom
                : symbol;

            FullyQualifiedName = nameSource.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            Name = nameSource.Name;
        }

        public bool Equals(TypeLookupInfo other) => FullyQualifiedName == other.FullyQualifiedName;
    }
}
