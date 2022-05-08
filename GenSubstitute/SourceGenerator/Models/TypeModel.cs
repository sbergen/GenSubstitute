using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeModel : IEquatable<TypeModel>
    {
        public readonly string FullyQualifiedName;
        public readonly string BuilderTypeName;
        public readonly ImmutableArray<string> TypeParameters;
        public readonly ImmutableArray<MethodModel> Methods;

        public TypeModel(INamedTypeSymbol symbol)
        {
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            BuilderTypeName = MakeBuilderName(symbol);
            
            var methodsBuilder = ImmutableArray.CreateBuilder<MethodModel>();
            GatherAllMethods(symbol, methodsBuilder);

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

        private static string MakeBuilderName(INamedTypeSymbol symbol) => string.Join(
            "_",
            symbol
                .ToDisplayParts()
                .TakeWhile(s => s.ToString() != "<")
                .Where(s => s.Kind != SymbolDisplayPartKind.Punctuation)
                .Append(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, "Builder")));

        private static void GatherAllMethods(
            INamedTypeSymbol symbol,
            ImmutableArray<MethodModel>.Builder builder)
        {
            static void AddMembers(INamedTypeSymbol type, ImmutableArray<MethodModel>.Builder builder)
            {
                foreach (var member in type.GetMembers())
                {
                    if (member is IMethodSymbol methodSymbol)
                    {
                        builder.Add(new MethodModel(methodSymbol));
                    }
                }
            }
            
            AddMembers(symbol, builder);
            foreach (var iface in symbol.AllInterfaces)
            {
                AddMembers(iface, builder);
            }
        }
    }
}
