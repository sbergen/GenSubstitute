using System;
using System.Linq;
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
                var typesSyntaxToMock = ((SyntaxReceiver)context.SyntaxReceiver!).TypesToMock;
                if (typesSyntaxToMock.Any())
                {
                    var mocks = typesSyntaxToMock
                        .Select(syntax => TypeSymbolResolver.TryResolve(context, syntax))
                        .Distinct(SymbolEqualityComparer.Default)
                        .Cast<INamedTypeSymbol>()
                        .Select(symbol => new MockInfo(symbol))
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
