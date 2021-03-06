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
#if UNITY
            GeneratorExecutionContext context,
#else
            SourceProductionContext context,
#endif
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

        public bool TryGetResult(out TResult result)
        {
            if (_isResult)
            {
                result = _result!;
                return true;
            }
            else
            {
                result = default!;
                return false;
            }
        }

        public ResultOrDiagnostic<TSelectResult> SelectMany<TSelectResult>(
            Func<TResult, ResultOrDiagnostic<TSelectResult>> selector)
            where TSelectResult : IEquatable<TSelectResult>
        {
            if (_isResult)
            {
                return selector(_result!);
            }
            else
            {
                return _diagnostic!;
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
