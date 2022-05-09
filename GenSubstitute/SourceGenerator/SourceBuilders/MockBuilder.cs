using System.Collections.Immutable;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class MockBuilder : SourceBuilder
    {
        public static string BuildMock(TypeModel model) => new MockBuilder(model).GetResult();

        private MockBuilder(TypeModel model)
        {
            var typeParameters = model.TypeParameters.IsEmpty ? "" : $"<{BuildList(model.TypeParameters)}>";

            AppendLine("internal static partial class GeneratorMarkerExtensions");
            AppendLine("{");
            using (Indent())
            {
                AppendLine(
                    $"public static {model.BuilderTypeName}{typeParameters} Build{typeParameters}(this GenerateMarker<{model.FullyQualifiedName}> m) => new();");
            }
            AppendLine("}");
            
            EmptyLine();
            
            AppendLine($"internal sealed class {model.BuilderTypeName}{typeParameters}");
            AppendLine("{");
            using (Indent())
            {
                BuildBuilderContents(model);
            }
            AppendLine("}");
        }

        private void BuildBuilderContents(TypeModel model)
        {
            var methods = model.Methods;
            var enrichedMethodInfo = ImmutableArray
                .CreateRange(model.Methods, m => new EnrichedMethodModel(m));

            ImplementationBuilder.Build(this, model, enrichedMethodInfo);
            EmptyLine();
            ReceivedCallsBuilder.Build(this, methods, enrichedMethodInfo);
            EmptyLine();
            ConfigurerBuilder.Build(this, methods, enrichedMethodInfo);
            
            EmptyLine();
            AppendLine($"private readonly {nameof(ConfiguredCalls)} _configuredCalls = new();");
            AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls = new();");
            AppendLine($"private readonly {ImplementationBuilder.ClassName} _implementation;");
            EmptyLine();
            AppendLine($"public {model.FullyQualifiedName} Object => _implementation;");
            AppendLine($"public {ReceivedCallsBuilder.ClassName} Received {{ get; }}");
            AppendLine($"public {ConfigurerBuilder.ClassName} Configure {{ get; }}");
            AppendLine($"public IReadOnlyList<{nameof(IReceivedCall)}> AllReceived => _receivedCalls.All;");
            EmptyLine();
            AppendLine($"internal {model.BuilderTypeName}()");
            AppendLine("{");
            using (Indent())
            {
                AppendLine("_implementation = new(_receivedCalls, _configuredCalls);");
                AppendLine("Received = new(_receivedCalls);");
                AppendLine("Configure = new(_configuredCalls);");
            }
            AppendLine("}");
        }
    }
}
