using System;
using System.Collections.Generic;
using System.Linq;
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
                    var extensionsBuilder = new GeneratorMarkerExtensionsBuilder();
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

                        if (maybeMock is {} mock && !includedMocks.Contains(mock.MockedTypeName))
                        {
                            includedMocks.Add(mock.MockedTypeName);
                            
                            extensionsBuilder.AddGenerateMethod(mock);
                            
                            context.AddSource(
                                $"{mock.BuilderTypeName}.cs",
                                MockBuilder.BuildMock(mock));
                        }
                    }
                    
                    context.AddSource(
                        "GenSubstitute_GeneratorMarkerExtensions.cs",
                        extensionsBuilder.GetResult());
                }
            }
            catch (Exception e)
            {
                // TODO, produce diagnostic, currently just working on a hunch...
                Console.WriteLine(e);
            }
        }
    }
}
