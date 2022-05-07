using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeModel : IEquatable<TypeModel>
    {
        public readonly string FullyQualifiedName;
        public readonly ImmutableArray<MethodModel> Methods;

        public TypeModel(INamedTypeSymbol symbol)
        {
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var methodsBuilder = ImmutableArray.CreateBuilder<MethodModel>();
            foreach (var member in symbol.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    methodsBuilder.Add(new MethodModel(methodSymbol));
                }
            }

            Methods = methodsBuilder.ToImmutable();
        }

        public bool Equals(TypeModel other) =>
            FullyQualifiedName == other.FullyQualifiedName &&
            Methods.SequenceEqual(other.Methods);
    }
}
