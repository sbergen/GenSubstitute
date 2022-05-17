using System.Linq;
using System.Threading;
using GenSubstitute.SourceGenerator.SourceBuilders;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    [Generator]
    internal class GenSubstituteGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SubstituteSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SubstituteSyntaxReceiver)context.SyntaxReceiver!;

            var typeInfo = syntaxReceiver.SubstituteCalls
                .Select(syntax => ModelExtractor.ExtractTypeInfoFromCompilationAndSyntax(
                    context.Compilation, syntax, CancellationToken.None))
                .ToList();
                
            var models = DuplicateFilter
                .FilterDuplicates(typeInfo, CancellationToken.None)
                .Select(type => ModelExtractor.ExtractModelFromCompilationAndName(
                    type, context.Compilation, CancellationToken.None));

            foreach (var modelOrDiagnostic in models)
            {
                modelOrDiagnostic.AddSourceOrDiagnostic(
                    context,
                    static model => ($"{model.SubstituteTypeName}.cs", MockBuilder.BuildMock(model)));
            }
        }
    }
}
