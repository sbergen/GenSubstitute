using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.Utilities;
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
        
        private static readonly DiagnosticDescriptor NotAnInterfaceDescriptor = new(
            id: "GENSUB002",
            title: "Substituted type is not an interface",
            messageFormat: "'{0}' is not an interface",
            category: "GenSubstitute",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        
        private static readonly DiagnosticDescriptor InternalErrorDescriptor = new(
            id: "GENSUB100",
            title: "Internal GenSubstitute error",
            messageFormat: "Something went wrong, please report this issue: {0}",
            category: "GenSubstitute",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static Diagnostic SymbolNotFound(TypeLookupInfo typeInfo) => Diagnostic.Create(
            SymbolNotFoundInCompilationDescriptor,
            Location.None,
            typeInfo.FullyQualifiedName);
        
        public static Diagnostic SymbolNotFound(TypeSyntax syntax) => Diagnostic.Create(
            SymbolNotFoundInSemanticModelDescriptor,
            syntax.GetLocation(),
            syntax.ToString());
        
        public static Diagnostic NotAnInterface(TypeSyntax syntax) => Diagnostic.Create(
            NotAnInterfaceDescriptor,
            syntax.GetLocation(),
            syntax.ToString());
        
        public static Diagnostic InternalError(string description, Location? location = null) => Diagnostic.Create(
            InternalErrorDescriptor,
            location ?? Location.None,
            description); 
    }
}
