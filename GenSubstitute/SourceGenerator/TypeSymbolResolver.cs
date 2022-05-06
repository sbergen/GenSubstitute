using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal static class TypeSymbolResolver
    {
        public static INamedTypeSymbol TryResolve(GeneratorExecutionContext context, TypeSyntax typeSyntax)
        {
            var semanticModel = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
            var symbol = semanticModel.GetSymbolInfo(typeSyntax).Symbol;
            
            // TOD check null first, report diagnostic
            if (symbol is not INamedTypeSymbol { TypeKind: TypeKind.Interface } typeSymbol)
            {
                // TODO report error
                return null;
            }
            else
            {
                return typeSymbol;  
            }
        }
    }
}
