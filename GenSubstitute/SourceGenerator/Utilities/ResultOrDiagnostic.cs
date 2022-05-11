using System;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Utilities
{
    public readonly struct ResultOrDiagnostic<TResult> : IEquatable<ResultOrDiagnostic<TResult>>
        where TResult : IEquatable<TResult>
    {
        private readonly bool _isResult;
        private readonly Diagnostic? _diagnostic;
        private readonly TResult? _result;
        
        public static implicit operator ResultOrDiagnostic<TResult>(TResult result) => new(result);
        public static implicit operator ResultOrDiagnostic<TResult>(Diagnostic diagnostic) => new(diagnostic);

        public void AddSourceOrDiagnostic(
            SourceProductionContext context,
            Func<TResult, (string hintName, string source)?> sourceGenerator)
        {
            if (_isResult)
            {
                if (sourceGenerator(_result!) is var (hintName, source))
                {
                    context.AddSource(hintName, source);
                }
            }
            else
            {
                context.ReportDiagnostic(_diagnostic!);
            }
        }
        
        private ResultOrDiagnostic(Diagnostic diagnostic)
        {
            _diagnostic = diagnostic;
            _result = default;
            _isResult = false;
        }

        private ResultOrDiagnostic(TResult result)
        {
            _diagnostic = null;
            _result = result;
            _isResult = true;
        }

        public bool Equals(ResultOrDiagnostic<TResult> other) =>
            (_isResult && other._isResult && _result!.Equals(other._result!)) ||
            (!_isResult && other._isResult && _diagnostic!.Equals(other._diagnostic!));
    }
}
