using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GenSubstitute.SourceGenerator.Utilities
{
    public readonly struct ResultOrDiagnostic<TResult> : IEquatable<ResultOrDiagnostic<TResult>>
        where TResult : struct, IEquatable<TResult>
    {
        private readonly Diagnostic? _diagnostic;
        
        public readonly TResult? Result;
        
        public static implicit operator ResultOrDiagnostic<TResult>(TResult result) => new(result);
        public static implicit operator ResultOrDiagnostic<TResult>(Diagnostic diagnostic) => new(diagnostic);

        public void AddSourceOrDiagnostic(
#if UNITY
            GeneratorExecutionContext context,
#else
            SourceProductionContext context,
#endif
            Func<TResult, (string hintName, SourceText source)?> sourceGenerator)
        {
            if (Result is {} result)
            {
                if (sourceGenerator(result) is var (hintName, source))
                {
                    context.AddSource(hintName, source);
                }
            }
            else
            {
                context.ReportDiagnostic(_diagnostic!);
            }
        }

        public ResultOrDiagnostic<TSelectResult> SelectMany<TSelectResult>(
            Func<TResult, ResultOrDiagnostic<TSelectResult>> selector)
            where TSelectResult : struct, IEquatable<TSelectResult> =>
            Result switch
            {
                {} result => selector(result),
                _ => _diagnostic!,
            };

        private ResultOrDiagnostic(Diagnostic diagnostic)
        {
            _diagnostic = diagnostic;
            Result = default;
        }

        private ResultOrDiagnostic(TResult result)
        {
            _diagnostic = null;
            Result = result;
        }

        public bool Equals(ResultOrDiagnostic<TResult> other) =>
            (Result, other.Result, _diagnostic, other._diagnostic) switch
            {
                ({} result, {} otherResult, _, _) => result.Equals(otherResult),
                (_, _, {} diagnostic, {} otherDiagnostic) => diagnostic.Equals(otherDiagnostic),
                _ => false,
            };
    }
}
