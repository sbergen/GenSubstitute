using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenSubstitute.SourceGenerator
{
    internal static class ModelExtractor
    {
        public static ResultOrDiagnostic<TypeLookupInfo> ExtractTypeNameFromSubstituteCall(
            GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (SyntaxFilter.ExtractTypeFromSubstituteCall(context.Node) is {} typeSyntax)
            {
                var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol is INamedTypeSymbol typeSymbol)
                {
                    // TODO: diagnostic, if not interface
                    return new TypeLookupInfo(typeSymbol);
                }
                else
                {
                    return Diagnostics.SymbolNotFound(typeSyntax);
                }
            }
            else
            {
                // TODO diagnostic
                return null;
            }
        }

        public static ResultOrDiagnostic<TypeModel> ExtractModelFromCompilationAndName(
            ResultOrDiagnostic<TypeLookupInfo> typeInfoOrDiagnostic,
            Compilation compilation,
            CancellationToken cancellationToken) => typeInfoOrDiagnostic
            .SelectMany<TypeModel>(typeInfo =>
                compilation.GetTypeByMetadataName(typeInfo.MetadataName) switch
                {
                    { } symbol => new TypeModel(symbol),
                    _ => Diagnostics.SymbolNotFound(typeInfo),
                });
    }
}
