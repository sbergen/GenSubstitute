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
            var typesSyntaxToMock = ((SyntaxReceiver)context.SyntaxReceiver!).TypesToMock;
            if (typesSyntaxToMock.Any())
            {
                var mocks = typesSyntaxToMock
                    .Select(syntax => TypeSymbolResolver.TryResolve(context, syntax))
                    .Where(symbol => symbol != null)
                    .Select(symbol => new MockInfo(symbol))
                    .ToList();
                
                var builder = new SourceBuilder();
                
                builder.GenerateGenExtensions(mocks);
                builder.GenerateBuilders(mocks);
                
                context.AddSource(OutputFileName, builder.Complete());
            }
        }
    }
}
