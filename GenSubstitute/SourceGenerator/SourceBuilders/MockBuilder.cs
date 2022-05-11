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
            Line("#nullable enable");
            EmptyLine();
            Line("using System.Collections.Generic;");
            EmptyLine();
            Line("namespace GenSubstitute");
            Line("{");
            
            using (Indent())
            {
                BuildNamespaceContents(model);
            }
            
            Line("}");
        }

        private void BuildNamespaceContents(TypeModel model)
        {
            var typeParameters = model.TypeParameters.IsEmpty ? "" : $"<{BuildList(model.TypeParameters)}>";

            Line("internal static partial class GeneratorMarkerExtensions");
            Line("{");
            using (Indent())
            {
                Line(
                    $"public static {model.BuilderTypeName}{typeParameters} Build{typeParameters}(this GenerateMarker<{model.FullyQualifiedName}> m) => new();");
            }
            Line("}");
            
            EmptyLine();
            
            Line($"internal sealed class {model.BuilderTypeName}{typeParameters}");
            Line("{");
            using (Indent())
            {
                BuildBuilderContents(model);
            }
            Line("}");
        }

        private void BuildBuilderContents(TypeModel model)
        {
            Line($"private readonly {nameof(ConfiguredCalls)} _configuredCalls = new();");
            Line($"private readonly {nameof(ReceivedCalls)} _receivedCalls = new();");
            Line($"private readonly {ImplementationBuilder.ClassName} _implementation;");
            EmptyLine();
            
            Line($"internal {model.BuilderTypeName}()");
            Line("{");
            using (Indent())
            {
                Line("_implementation = new(_receivedCalls, _configuredCalls);");
                Line("Received = new(_receivedCalls);");
                Line("Configure = new(_configuredCalls);");
            }

            Line("}");
            
            var enrichedMethods = ImmutableArray
                .CreateRange(model.Methods, m => new EnrichedMethodModel(m));

            EmptyLine();
            ImplementationBuilder.Build(this, model, enrichedMethods);
            EmptyLine();
            ReceivedCallsBuilder.Build(this, enrichedMethods);
            EmptyLine();
            ConfigurerBuilder.Build(this, enrichedMethods);
            EmptyLine();
            
            Line($"public {model.FullyQualifiedName} Object => _implementation;");
            Line($"public {ReceivedCallsBuilder.ClassName} Received {{ get; }}");
            Line($"public {ConfigurerBuilder.ClassName} Configure {{ get; }}");
            Line($"public IReadOnlyList<{nameof(IReceivedCall)}> AllReceived => _receivedCalls.All;");
        }
    }
}
