using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeModel : IEquatable<TypeModel>
    {
        public readonly string MockedTypeName;
        public readonly string BuilderTypeName;
        public readonly ImmutableArray<MethodModel> Methods;

        public TypeModel(INamedTypeSymbol symbol)
        {
            MockedTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var nameValidForType = MockedTypeName
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("<", "_")
                .Replace(">", "_");
            
            BuilderTypeName = $"{nameof(GenSubstitute)}_{nameValidForType}_Builder";

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

        public bool Equals(TypeModel other) => MockedTypeName == other.MockedTypeName;
    }
}
