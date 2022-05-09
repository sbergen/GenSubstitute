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
            ReceivedCallsBuilder.Build(this, methods, enrichedMethodInfo);
            
            EmptyLine();
            AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls = new();");
            AppendLine($"private readonly {ImplementationBuilder.ClassName} _implementation;");
            EmptyLine();
            AppendLine($"public {model.FullyQualifiedName} Object => _implementation;");
            AppendLine($"public {ReceivedCallsBuilder.ClassName} Received {{ get; }}");
            AppendLine($"public IReadOnlyList<{nameof(IReceivedCall)}> AllReceived => _receivedCalls.All;");
            
            AppendLine($"internal {model.BuilderTypeName}()");
            AppendLine("{");
            using (Indent())
            {
                AppendLine("_implementation = new(_receivedCalls);");
                AppendLine("Received = new(_receivedCalls);");
            }
            AppendLine("}");
            
            for (var i = 0; i < methods.Length; ++i)
            {
                EmptyLine();
                BuildConfigureMethod(methods[i], enrichedMethodInfo[i]);
            }
        }

        private void BuildConfigureMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            AppendLine($"public {enriched.ConfiguredCallType} {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_implementation.ConfiguredCalls.Add({enriched.ResolvedMethodName}, new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
            }
        }
    }
}
