using System;
using System.Collections.Generic;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator
{
    [Generator]
    internal class MockGenerator : ISourceGenerator
    {
        internal const string OutputFileName = "GenSubstituteMocks.cs";
        
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
                    var mocks = new List<TypeModel>();
                    var includedMocks = new HashSet<string>();
                    foreach (var syntax in generateCalls)
                    {
                        var mock = ModelExtractor.ExtractMockModelFromSubstituteCall(
                            syntax,
                            context.Compilation.GetSemanticModel(syntax.SyntaxTree),
                            context.CancellationToken);

                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (mock != null && !includedMocks.Contains(mock.Value.MockedTypeName))
                        {
                            includedMocks.Add(mock.Value.MockedTypeName);
                            mocks.Add(mock.Value);
                        }
                    }

                    var builder = new SourceBuilder();
                
                    builder.GenerateGenExtensions(mocks);

                    foreach (var mock in mocks)
                    {
                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        builder.GenerateBuilder(mock);
                    }

                    context.AddSource(OutputFileName, builder.Complete());
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
