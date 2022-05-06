using System.Collections.Generic;
using System.Text;

namespace GenSubstitute.SourceGenerator
{
    internal class SourceBuilder
    {
        private readonly StringBuilder _result = new();

        public SourceBuilder()
        {
            _result.AppendLine("namespace GenSubstitute");
            _result.AppendLine("{");
        }

        public string Complete()
        {
            _result.AppendLine("}");
            return _result.ToString();
        }

        public void GenerateGenExtensions(IEnumerable<MockInfo> mocks)
        {
            _result.AppendLine("  internal static class GenExtensions");
            _result.AppendLine("  {");

            foreach (var mock in mocks)
            {
                GenerateConfigureMethod(mock);
            }
            
            _result.AppendLine("  }");
        }

        private void GenerateConfigureMethod(MockInfo mockInfo)
        {
            _result.AppendLine(
                $"    public static object Configure(this GenerateMarker<{mockInfo.FullyQualifiedName}> m) => new();");
        }
    }
}
