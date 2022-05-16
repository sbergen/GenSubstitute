using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.SourceBuilders;
using GenSubstitute.SourceGenerator.Utilities;
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
                .SelectMany((data, ct) => DuplicateFilter
                    .FilterDuplicates(data, ct))
                .Combine(context.CompilationProvider)
                .Select((data, ct) => ModelExtractor
                    .ExtractModelFromCompilationAndName(data.Left, data.Right, ct));

            context.RegisterSourceOutput(models, (spContext, modelOrDiagnostic) =>
            {
                modelOrDiagnostic.AddSourceOrDiagnostic(
                    spContext,
                    static model => ($"{model.SubstituteTypeName}.cs", MockBuilder.BuildMock(model)));
            });
        }
    }
}
