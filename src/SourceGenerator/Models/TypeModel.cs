using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct TypeModel : IEquatable<TypeModel>
    {
        public readonly string MinimallyQualifiedName;
        public readonly string FullyQualifiedName;
        public readonly string SubstituteTypeName;
        public readonly ImmutableArray<string> TypeParameters;
        public readonly ImmutableArray<MethodModel> Methods;
        public readonly ImmutableArray<PropertyModel> Properties;
        public readonly ImmutableArray<EventModel> Events;

        public TypeModel(INamedTypeSymbol symbol)
        {
            MinimallyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            SubstituteTypeName = MakeSubstituteName(symbol);
            
            var methodsBuilder = ImmutableArray.CreateBuilder<MethodModel>();
            var propertiesBuilder = ImmutableArray.CreateBuilder<PropertyModel>();
            var eventsBuilder = ImmutableArray.CreateBuilder<EventModel>();
            GatherAllMethodsAndProperties(symbol, methodsBuilder, propertiesBuilder, eventsBuilder);
            
            Methods = methodsBuilder.ToImmutable();
            Properties = propertiesBuilder.ToImmutable();
            Events = eventsBuilder.ToImmutable();

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
            Properties.SequenceEqual(other.Properties) &&
            Events.SequenceEqual(other.Events);

        private static string MakeSubstituteName(INamedTypeSymbol symbol) => string.Join(
            "_",
            symbol
                .ToDisplayParts()
                .TakeWhile(s => s.ToString() != "<")
                .Where(s => s.Kind != SymbolDisplayPartKind.Punctuation)
                .Append(new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, "Substitute")));

        private static void GatherAllMethodsAndProperties(
            INamedTypeSymbol symbol,
            ImmutableArray<MethodModel>.Builder methodsBuilder,
            ImmutableArray<PropertyModel>.Builder propertiesBuilder,
            ImmutableArray<EventModel>.Builder eventsBuilder)
        {
            void AddMembers(INamedTypeSymbol type)
            {
                foreach (var member in type.GetMembers())
                {
                    // Skip property and event methods (and any special methods in the future)
                    if (member is IMethodSymbol { AssociatedSymbol: null } methodSymbol)
                    {
                        methodsBuilder.Add(new(methodSymbol));
                    }
                    else if (member is IPropertySymbol propertySymbol)
                    {
                        propertiesBuilder.Add(new(propertySymbol));
                    }
                    else if (member is IEventSymbol eventSymbol)
                    {
                        eventsBuilder.Add(new(eventSymbol));
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
