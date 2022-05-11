using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenSubstitute.SourceGenerator
{
    internal static class Diagnostics
    {
        private static readonly DiagnosticDescriptor SymbolNotFoundInCompilationDescriptor = new(
            id: "GENSUB001",
            title: "Couldn't find symbol",
            messageFormat: "Couldn't find symbol '{0}' in compilation.",
            category: "GenSubstitute",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        
        private static readonly DiagnosticDescriptor SymbolNotFoundInSemanticModelDescriptor = new(
            id: "GENSUB002",
            title: "Couldn't find symbol",
            messageFormat: "Couldn't find symbol '{0}' in semantic model.",
            category: "GenSubstitute",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static Diagnostic SymbolNotFound(TypeLookupInfo typeInfo) => Diagnostic.Create(
            SymbolNotFoundInCompilationDescriptor,
            Location.None,
            typeInfo.FullyQualifiedName);
        
        public static Diagnostic SymbolNotFound(TypeSyntax syntax) => Diagnostic.Create(
            SymbolNotFoundInSemanticModelDescriptor,
            Location.None,
            syntax.ToString());
    }
}
