using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    internal sealed class MockParameterInfo
    {
        public readonly string Type;
        public readonly string Name;
        
        public MockParameterInfo(IParameterSymbol symbol)
        {
            Type = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            Name = symbol.Name;
;        }
    }
}
