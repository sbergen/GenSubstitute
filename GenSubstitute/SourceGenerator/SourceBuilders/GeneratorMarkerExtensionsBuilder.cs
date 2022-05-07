using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class GeneratorMarkerExtensionsBuilder : SourceBuilder
    {
        private readonly IndentationScope _indent;
        
        public GeneratorMarkerExtensionsBuilder()
        {
            AppendLine("internal static class GeneratorMarkerExtensions");
            AppendLine("{");
            _indent = Indent();
        }
        
        protected override void GenerateEnd()
        {
            _indent.Dispose();
            AppendLine("}");
        }

        public void AddGenerateMethod(TypeModel model) => AppendLine(
            $"public static {model.BuilderTypeName} Build(this GenerateMarker<{model.MockedTypeName}> m) => new();");
    }
}
