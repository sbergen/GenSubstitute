using GenSubstitute.Internal;
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
            Line("using GenSubstitute.Internal;");
            EmptyLine();
            Line("namespace GenSubstitute");
            Line("{");
            
            using (Indent())
            {
                BuildNamespaceContents(model);
            }
            
            Line("}");
        }

        protected override void FinalizeContent()
        {
            // Nothing to do
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
            Line($"private readonly {nameof(ISubstitutionContext)} _context;");
            Line($"private readonly {ImplementationBuilder.ClassName} _implementation;");
            EmptyLine();
            
            Line($"public {model.FullyQualifiedName} Object => _implementation;");
            Line($"public {ReceivedCallsBuilder.ClassName} Received {{ get; }}");
            Line($"public {ConfigurerBuilder.ClassName} Configure {{ get; }}");
            Line($"public IReadOnlyList<{nameof(IReceivedCall)}> AllReceived => _context.Received.All;");
            EmptyLine();
            
            Line($"internal {model.BuilderTypeName}({nameof(SubstitutionContext)}? context = null)");
            Line("{");
            using (Indent())
            {
                Line("_context = context ?? new();");
                Line("_implementation = new(_context);");
                Line("Received = new(_context);");
                Line("Configure = new(_context);");
            }

            Line("}");

            var implementationBuilder = new ImplementationBuilder(this, model);
            var receivedBuilder = new ReceivedCallsBuilder(this);
            var configurerBuilder = new ConfigurerBuilder(this);

            foreach (var property in model.Properties)
            {
                implementationBuilder.AddProperty(property);
                receivedBuilder.AddProperty(property);
                configurerBuilder.AddProperty(property);
            }
            
            foreach (var method in model.Methods)
            {
                var enrichedModel = new EnrichedMethodModel(method);
                
                implementationBuilder.AddMethod(enrichedModel);
                receivedBuilder.AddMethod(enrichedModel);
                configurerBuilder.AddMethod(enrichedModel);
            }

            EmptyLine();
            AppendWithoutIndent(implementationBuilder.GetResult());
            EmptyLine();
            AppendWithoutIndent(receivedBuilder.GetResult());
            EmptyLine();
            AppendWithoutIndent(configurerBuilder.GetResult());
        }
    }
}
