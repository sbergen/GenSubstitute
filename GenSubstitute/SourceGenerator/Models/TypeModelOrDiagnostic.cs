using System;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    // TODO consider using some OneOf library here...
    internal readonly struct TypeModelOrDiagnostic : IEquatable<TypeModelOrDiagnostic>
    {
        public readonly Diagnostic Diagnostic;
        public readonly TypeModel? Model;

        public static implicit operator TypeModelOrDiagnostic(TypeModel model) => new(model);
        public static implicit operator TypeModelOrDiagnostic(Diagnostic diagnostic) => new(diagnostic);
        
        public TypeModelOrDiagnostic(Diagnostic diagnostic)
        {
            Diagnostic = diagnostic;
            Model = null;
        }

        public TypeModelOrDiagnostic(TypeModel model)
        {
            Diagnostic = null;
            Model = model;
        }

        public bool Equals(TypeModelOrDiagnostic other) =>
            (Diagnostic != null && other.Diagnostic != null && Diagnostic.Equals(other.Diagnostic)) ||
            (Model != null && other.Model != null && Model.Equals(other.Model));
    }
}
