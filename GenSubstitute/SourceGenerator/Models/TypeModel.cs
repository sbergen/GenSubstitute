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
        public readonly ImmutableArray<PropertyModel> Properties;

        public TypeModel(INamedTypeSymbol symbol)
        {
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            BuilderTypeName = MakeBuilderName(symbol);
            
            var methodsBuilder = ImmutableArray.CreateBuilder<MethodModel>();
            var propertiesBuilder = ImmutableArray.CreateBuilder<PropertyModel>();
            GatherAllMethodsAndProperties(symbol, methodsBuilder, propertiesBuilder);
            
            Methods = methodsBuilder.ToImmutable();
            Properties = propertiesBuilder.ToImmutable();

            var typeParametersBuilder = ImmutableArray.CreateBuilder<string>(symbol.TypeParameters.Length);
            foreach (var typeParameter in symbol.TypeParameters)
            {
                typeParametersBuilder.Add(typeParameter.Name);
            }

            TypeParameters = typeParametersBuilder.ToImmutable();
        }

        public bool Equals(TypeModel other) =>
            // Fully qualified name already includes type parameters
            FullyQualifiedName == other.FullyQualifiedName &&
            Methods.SequenceEqual(other.Methods) &&
            Properties.SequenceEqual(other.Properties);

        private static string MakeBuilderName(INamedTypeSymbol symbol) => string.Join(
            "_",
            symbol
                .ToDisplayParts()
                .TakeWhile(s => s.ToString() != "<")
                .Where(s => s.Kind != SymbolDisplayPartKind.Punctuation)
                .Append(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, "Builder")));

        private static void GatherAllMethodsAndProperties(
            INamedTypeSymbol symbol,
            ImmutableArray<MethodModel>.Builder methodsBuilder,
            ImmutableArray<PropertyModel>.Builder propertiesBuilder)
        {
            void AddMembers(INamedTypeSymbol type)
            {
                foreach (var member in type.GetMembers())
                {
                    if (member is IMethodSymbol methodSymbol)
                    {
                        methodsBuilder.Add(new MethodModel(methodSymbol));
                    }
                    else if (member is IPropertySymbol propertySymbol)
                    {
                        propertiesBuilder.Add(new PropertyModel(propertySymbol));
                    }
                }
            }
            
            AddMembers(symbol);
            foreach (var iface in symbol.AllInterfaces)
            {
                AddMembers(iface);
            }
        }
    }
}
