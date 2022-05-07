using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeModel : IEquatable<TypeModel>
    {
        public readonly string FullyQualifiedName;
        public readonly ImmutableArray<string> TypeParameters;
        public readonly ImmutableArray<MethodModel> Methods;

        public TypeModel(INamedTypeSymbol symbol)
        {
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var members = symbol.GetMembers();
            var methodsBuilder = ImmutableArray.CreateBuilder<MethodModel>(members.Length);
            foreach (var member in members)
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    methodsBuilder.Add(new MethodModel(methodSymbol));
                }
            }

            var typeParametersBuilder = ImmutableArray.CreateBuilder<string>(symbol.TypeParameters.Length);
            foreach (var typeParameter in symbol.TypeParameters)
            {
                typeParametersBuilder.Add(typeParameter.Name);
            }

            TypeParameters = typeParametersBuilder.ToImmutable();

            Methods = methodsBuilder.ToImmutable();
        }

        public bool Equals(TypeModel other) =>
            FullyQualifiedName == other.FullyQualifiedName &&
            Methods.SequenceEqual(other.Methods);
    }
}
