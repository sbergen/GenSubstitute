using System.Collections.Generic;
using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.Utilities;

namespace GenSubstitute.SourceGenerator
{
    internal static class DuplicateFilter
    {
        public static IEnumerable<ResultOrDiagnostic<TypeLookupInfo>> FilterDuplicates(
            IEnumerable<ResultOrDiagnostic<TypeLookupInfo>> candidates,
            CancellationToken cancellationToken)
        {
            var includedMocks = new HashSet<string>();

            foreach (var typeInfoOrDiagnostic in candidates)
            {
                if (typeInfoOrDiagnostic.Result is {} result)
                {
                    if (!includedMocks.Contains(result.FullyQualifiedName))
                    {
                        includedMocks.Add(result.FullyQualifiedName);
                        yield return result;
                    }
                }
                else
                {
                    yield return typeInfoOrDiagnostic;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
