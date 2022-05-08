using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.SourceBuilders;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    [Generator]
    internal class GenSubstituteGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var models = context.SyntaxProvider
                .CreateSyntaxProvider(
                    SyntaxFilter.IsSubstituteCall,
                    ModelExtractor.ExtractTypeNameFromSubstituteCall)
                .Collect()
                .SelectMany(FilterNullsAndDuplicates)
                .Combine(context.CompilationProvider)
                .Select((data, ct) => ModelExtractor
                    .ExtractModelFromCompilationAndName(data.Left, data.Right, ct));

            context.RegisterSourceOutput(models, (spContext, modelOrDiagnostic) =>
            {
                if (modelOrDiagnostic is { Model: {} model })
                {
                    spContext.AddSource(
                        $"{model.BuilderTypeName}.cs",
                        MockBuilder.BuildMock(model));
                }
                else if (modelOrDiagnostic is { Diagnostic: {} diagnostic })
                {
                    spContext.ReportDiagnostic(diagnostic);
                }
            });
        }

        private static IEnumerable<TypeLookupInfo> FilterNullsAndDuplicates(
            ImmutableArray<TypeLookupInfo?> candidates,
            CancellationToken cancellationToken)
        {
            var includedMocks = new HashSet<string>();

            foreach (var maybeCandidate in candidates)
            {
                if (maybeCandidate is {} candidate &&
                    !includedMocks.Contains(candidate.FullyQualifiedName))
                {
                    includedMocks.Add(candidate.FullyQualifiedName);
                    yield return candidate;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
