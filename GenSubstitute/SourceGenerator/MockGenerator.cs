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
                    var mocks = generateCalls
                        .Select(syntax => ModelExtractor.ExtractMockModelFromSubstituteCall(
                            syntax, context.Compilation.GetSemanticModel(syntax.SyntaxTree)))
                        .Where(model => model != null)
                        .Select(m => m.Value)
                        .Distinct(EqualityComparer<TypeModel>.Default)
                        .ToList();
                
                    var builder = new SourceBuilder();
                
                    builder.GenerateGenExtensions(mocks);
                    builder.GenerateBuilders(mocks);
                
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
