namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class ClassBuilder : SourceBuilder
    {
        private IndentationScope _indent;
        
        protected ClassBuilder(string classDeclaration)
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
