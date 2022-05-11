namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class ClassBuilder : SourceBuilder
    {
        private readonly IndentationScope _indent;

        protected ClassBuilder(SourceBuilder parent, string classDeclaration)
            : base(parent)
        {
            Line(classDeclaration);
            Line("{");
            _indent = Indent();
        }
        
        protected sealed override void FinalizeContent()
        {
            _indent.Dispose();
            Line("}");
        }
    }
}
