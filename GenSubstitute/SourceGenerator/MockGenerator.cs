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
        internal const string MocksFileName = "GenSubstituteMocks.cs";
        internal const string ExtensionsFileName = "GenSubstitute_GeneratorMarkerExtensions.cs";
        
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
                    var mocksBuilder = new SourceBuilder();
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
                            mocksBuilder.GenerateBuilder(mock);
                        }
                    }
                    
                    context.AddSource(ExtensionsFileName, extensionsBuilder.GetResult());
                    context.AddSource(MocksFileName, mocksBuilder.Complete());
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
