using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    // Model this to be compatible with incremental generators later.
    internal static class ModelExtractor
    {
        // The arguments are essentially what is contained in GeneratorSyntaxContext
        public static TypeModel? ExtractMockModelFromSubstituteCall(
            SyntaxNode node,
            SemanticModel nodeModel,
            CancellationToken cancellationToken)
        {
            if (SyntaxFilter.ExtractTypeFromSubstituteCall(node) is {} typeSyntax)
            {
                var semanticModel = nodeModel.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol is INamedTypeSymbol typeSymbol)
                {
                    return new TypeModel(typeSymbol);
                }
                else
                {
                    // TODO diagnostic
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
