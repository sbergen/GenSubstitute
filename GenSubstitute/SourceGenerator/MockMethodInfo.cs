using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    internal sealed class MockMethodInfo
    {
        public readonly bool ReturnsVoid;
        public readonly string ReturnType;
        public readonly string Name;
        public readonly IReadOnlyList<MockParameterInfo> Parameters;
        public readonly string ConfiguredCallType;

        public MockMethodInfo(IMethodSymbol symbol)
        {
            ReturnsVoid = symbol.ReturnType.SpecialType == SpecialType.System_Void;
            ReturnType = ReturnsVoid
                ? "void"
                : symbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var generics = "";
            if (symbol.IsGenericMethod)
            {
                generics = $"<{string.Join(", ", symbol.TypeParameters.Select(t => t.Name))}>";
            }
            
            Name = symbol.Name + generics;
            Parameters = symbol.Parameters.Select(p => new MockParameterInfo(p)).ToList();
            
            // TODO, this is a bit sketchy!
            ConfiguredCallType = ConfiguredCallBuilder.Make(this);
        }
    }
}
