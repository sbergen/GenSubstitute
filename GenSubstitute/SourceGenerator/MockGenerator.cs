using System;
using System.Collections.Generic;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.SourceBuilders;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    [Generator]
    internal class MockGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var generateCalls = ((SyntaxReceiver)context.SyntaxReceiver!).GenerateCalls;
                if (generateCalls.Any())
                {
                    var includedMocks = new HashSet<string>();
                    
                    foreach (var syntax in generateCalls)
                    {
                        var maybeMock = ModelExtractor.ExtractMockModelFromSubstituteCall(
                            syntax,
                            context.Compilation.GetSemanticModel(syntax.SyntaxTree),
                            context.CancellationToken);

                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (maybeMock is {} mock && !includedMocks.Contains(mock.FullyQualifiedName))
                        {
                            includedMocks.Add(mock.FullyQualifiedName);

                            var builderName = MakeBuilderName(mock);
                            context.AddSource(
                                $"{builderName}.cs",
                                MockBuilder.BuildMock(mock, builderName));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // TODO, produce diagnostic, currently just working on a hunch...
                Console.WriteLine(e);
            }
        }

        private static string MakeBuilderName(TypeModel model)
        {
            var nameValidForType = model.FullyQualifiedName
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("<", "_")
                .Replace(">", "_");

            return $"{nameValidForType}_Builder";
        }
    }
}
