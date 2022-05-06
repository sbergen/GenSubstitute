using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    internal sealed class MockInfo
    {
        public readonly string MockedTypeName;
        public readonly string BuilderTypeName;
        public readonly IReadOnlyList<MockMethodInfo> Methods;

        public MockInfo(INamedTypeSymbol symbol)
        {
            MockedTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var nameValidForType = MockedTypeName
                .Replace(".", "_")
                .Replace("global::", "")
                .Replace("<", "_")
                .Replace(">", "_");
            
            BuilderTypeName = $"{nameof(GenSubstitute)}_{nameValidForType}_Builder";

            var methods = new List<MockMethodInfo>();
            foreach (var member in symbol.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    methods.Add(new MockMethodInfo(methodSymbol));
                }
            }

            Methods = methods;
        }
    }
}
