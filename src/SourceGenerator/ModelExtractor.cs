using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal static class ModelExtractor
    {
#if !UNITY
        public static ResultOrDiagnostic<TypeLookupInfo> ExtractTypeNameFromSubstituteCall(
            GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (SyntaxFilter.ExtractTypeFromSubstituteCall(context.Node) is {} typeSyntax)
            {
                return ExtractTypeInfoFromCompilationAndSyntax(
                    context.SemanticModel.Compilation,
                    typeSyntax,
                    cancellationToken);
            }
            else
            {
                return Diagnostics.InternalError(
                    "Could not resolve type from substitute call",
                    context.Node.GetLocation());
            }
        }
#endif

        public static ResultOrDiagnostic<TypeLookupInfo> ExtractTypeInfoFromCompilationAndSyntax(
            Compilation compilation,
            TypeSyntax typeSyntax,
            CancellationToken cancellationToken)
        {
            var semanticModel = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
            if (semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol is INamedTypeSymbol typeSymbol)
            {
                return typeSymbol.TypeKind == TypeKind.Interface
                    ? new TypeLookupInfo(typeSymbol)
                    : Diagnostics.NotAnInterface(typeSyntax);
            }
            else
            {
                return Diagnostics.SymbolNotFound(typeSyntax);
            }
        }

        public static ResultOrDiagnostic<TypeModel> ExtractModelFromCompilationAndName(
            ResultOrDiagnostic<TypeLookupInfo> typeInfoOrDiagnostic,
            Compilation compilation,
            CancellationToken cancellationToken) => typeInfoOrDiagnostic
            .SelectMany<TypeModel>(typeInfo =>
                // Using GetTypesByMetadataName(...).FirstOrDefault here did not work for whatever reason in Rider,
                // but works in builds. Need to debug this more later.
                compilation.GetTypeByMetadataName(typeInfo.MetadataName) switch
                {
                    { } symbol => new TypeModel(symbol),
                    _ => Diagnostics.SymbolNotFound(typeInfo),
                });
    }
}
