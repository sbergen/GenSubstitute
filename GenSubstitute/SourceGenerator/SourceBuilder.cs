using System.Collections.Generic;
using System.Linq;
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

        public void GenerateBuilders(IEnumerable<MockInfo> mocks)
        {
            foreach (var mock in mocks)
            {
                _result.AppendLine($"   public sealed class {mock.BuilderTypeName}");
                _result.AppendLine("    {");
                _result.AppendLine($"      private class Implementation : {mock.MockedTypeName}");
                _result.AppendLine("      {");
                _result.AppendLine("        public readonly ConfiguredCalls Calls = new();");

                foreach (var method in mock.Methods)
                {
                    var parametersWithTypes = string.Join(", ", method.Parameters
                        .Select(p => $"{p.Type} {p.Name}"));
                    
                    var parameterNames = string.Join(", ", method.Parameters.Select(p => p.Name));

                    _result.AppendLine();
                    _result.AppendLine($"        public {method.ReturnType} {method.Name}({parametersWithTypes}) => Calls");
                    _result.AppendLine($"          .Get<{method.ConfiguredCallType}>(");
                    _result.AppendLine($"            typeof({method.ReturnType}),");
                    _result.AppendLine("            new TypeValuePair[]");
                    _result.AppendLine("            {");
                    foreach (var p in method.Parameters)
                    {
                        _result.AppendLine($"              {nameof(TypeValuePair)}.Make({p.Name}),");
                    }
                    _result.AppendLine("            })");
                    _result.AppendLine(method.ReturnsVoid
                        ? $"          .Execute({parameterNames});"
                        : $"          ?.Execute({parameterNames}) ?? default;");
                }
                
                _result.AppendLine("      }");

                _result.AppendLine();
                _result.AppendLine("      private readonly Implementation _implementation = new();");
                _result.AppendLine();

                foreach (var method in mock.Methods)
                {
                    var parametersWithTypes = string.Join(", ", method.Parameters
                        .Select(p => $"Arg<{p.Type}> {p.Name}"));
                    
                    // TODO duplication
                    var parameterNames = string.Join(", ", method.Parameters.Select(p => p.Name));
                
                    _result.AppendLine();
                    _result.AppendLine($"      {method.ConfiguredCallType} {method.Name}({parametersWithTypes}) =>");
                    _result.AppendLine($"        _implementation.Calls.Add(new {method.ConfiguredCallType}({parameterNames}));");
                }
                
                _result.AppendLine("    }");
            }
        }

        private void GenerateConfigureMethod(MockInfo mockInfo)
        {
            _result.AppendLine(
                $"    public static {mockInfo.BuilderTypeName} Configure(this GenerateMarker<{mockInfo.MockedTypeName}> m) => new();");
        }
    }
}
