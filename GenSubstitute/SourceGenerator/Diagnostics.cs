using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    internal static class Diagnostics
    {
        private static readonly DiagnosticDescriptor SymbolNotFoundDescriptor = new(
            id: "GENSUB001",
            title: "Couldn't find symbol",
            messageFormat: "Couldn't find symbol '{0}' in compilation.",
            category: "GenSubstitute",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static Diagnostic SymbolNotFound(TypeLookupInfo typeInfo) => Diagnostic.Create(
            SymbolNotFoundDescriptor,
            Location.None,
            typeInfo.FullyQualifiedName);
    }
}
