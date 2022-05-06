using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    internal class MockInfo
    {
        public readonly string FullyQualifiedName;
        
        public MockInfo(INamedTypeSymbol symbol)
        {
            FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
