using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.SourceBuilders;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    [Generator]
    internal class MockGenerator : IIncrementalGenerator
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

            context.RegisterSourceOutput(models, (spContext, maybeModel) =>
            {
                if (maybeModel is {} model)
                {
                    var builderName = MakeBuilderName(model);
                    spContext.AddSource(
                        $"{builderName}.cs",
                        MockBuilder.BuildMock(model, builderName));
                }
            });
        }

        private static string MakeBuilderName(TypeModel model)
        {
            // TODO, build better names during aggregation?
            var nameValidForType = model.FullyQualifiedName
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace(",", "_")
                .Replace("<", "_")
                .Replace(">", "");

            return $"{nameValidForType}_Builder";
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

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
    }
}
