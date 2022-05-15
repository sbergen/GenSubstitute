using System.Collections.Generic;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class ClassBuilder : SourceBuilder
    {
        private readonly List<string> _constructorLines = new();
        private readonly IndentationScope _indent;
        private readonly string _className;
        private readonly string _constructorArgs;

        protected ClassBuilder(
            SourceBuilder parent,
            string className,
            string constructorArgs,
            string? inheritance = null)
            : base(parent)
        {
            _className = className;
            _constructorArgs = constructorArgs;

            Line($"public class {className}{inheritance}");
            Line("{");
            _indent = Indent();
        }

        // "queues" a line to be added to the constructor
        protected void ConstructorLine(string line) => _constructorLines.Add(line);
        
        protected override void FinalizeContent()
        {
            EmptyLine();
            Line($"internal {_className}({_constructorArgs})");
            Line("{");
            using (Indent())
            {
                foreach (var line in _constructorLines)
                {
                    Line(line);
                }
            }
            Line("}");
            
            _indent.Dispose();
            Line("}");
        }
    }
}
