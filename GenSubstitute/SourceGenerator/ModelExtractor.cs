using System.Linq;
using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.SourceGenerator
{
    internal static class ModelExtractor
    {
        public static TypeLookupInfo? ExtractTypeNameFromSubstituteCall(
            GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (SyntaxFilter.ExtractTypeFromSubstituteCall(context.Node) is {} typeSyntax)
            {
                var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol is INamedTypeSymbol typeSymbol)
                {
                    return new TypeLookupInfo(typeSymbol);
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

        public static TypeModel? ExtractModelFromCompilationAndName(
            TypeLookupInfo typeInfo,
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var symbol = compilation
                .GetSymbolsWithName(typeInfo.Name, SymbolFilter.Type, cancellationToken)
                .OfType<INamedTypeSymbol>()
                .SingleOrDefault(symbol => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == typeInfo.FullyQualifiedName);
            
            if (symbol != null)
            {
                return new TypeModel(symbol);
            }
            else
            {
                // TODO diagnostic
                return null;
            }
        }
    }
}
