using Microsoft.CodeAnalysis;

namespace GenSubstitute
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
            var typesToMock = ((SyntaxReceiver)context.SyntaxReceiver!).TypesToMock;
            context.AddSource(OutputFileName, "// FOO");
        }
    }
}
