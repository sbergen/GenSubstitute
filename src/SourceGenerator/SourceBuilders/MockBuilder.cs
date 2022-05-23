using System.Linq;
using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringUtils;

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
                Line($"public static {model.SubstituteTypeName}{typeParameters} Create{typeParameters}(");
                using (Indent())
                {
                    Line($"this GenerateMarker<{model.FullyQualifiedName}> m,");
                    Line("SubstitutionContext? context = null,");
                    Line("string? identifier = null)");
                    Line("=> new(context, identifier);");
                }
            }
            Line("}");
            
            EmptyLine();
            
            Line($"internal sealed class {model.SubstituteTypeName}{typeParameters} : {nameof(ISubstitute)}");
            Line("{");
            using (Indent())
            {
                BuildBuilderContents(model);
            }
            Line("}");
        }

        private void BuildBuilderContents(TypeModel model)
        {
            Line($"private readonly {nameof(ObjectSubstitutionContext)} _context;");
            Line($"private readonly {ImplementationBuilder.ClassName} _implementation;");
            EmptyLine();
            
            Line($"public string {nameof(ISubstitute.Identifier)} {{ get; }}");
            Line($"public {model.FullyQualifiedName} Object => _implementation;");
            Line($"public {ReceivedCallsBuilder.ClassName} Received {{ get; }}");
            Line($"public {ConfigurerBuilder.ClassName} SetUp {{ get; }}");
            Line($"public {MatchersBuilder.ClassName} Match {{ get; }}");
            Line($"public IEnumerable<{nameof(IReceivedCall)}> AllReceived => _context.Received.ForSubstitute(this);");

            EventRaiserBuilder? eventRaiserBuilder = model.Events.Any() ? new(this) : null;
            if (eventRaiserBuilder != null)
            {
                Line($"public {EventRaiserBuilder.ClassName} Raise {{ get; }}");
            }

            EmptyLine();

            Line($"internal {model.SubstituteTypeName}(");
            using (Indent())
            {
                Line($"{nameof(ISubstitutionContext)}? context = null,");
                Line("string? identifier = null)");
            }
            Line("{");
            using (Indent())
            {
                Line($"Identifier = identifier ?? \"{model.MinimallyQualifiedName}\";");
                Line($"_context = new(this, context ?? new {nameof(SubstitutionContext)}());");
                Line("_implementation = new(_context);");
                Line("Received = new(_context);");
                Line("SetUp = new(_context);");
                Line("Match = new(_context);");

                if (eventRaiserBuilder != null)
                {
                    Line("Raise = new(_implementation);");
                }
            }

            Line("}");

            var implementationBuilder = new ImplementationBuilder(this, model);
            var receivedBuilder = new ReceivedCallsBuilder(this);
            var configurerBuilder = new ConfigurerBuilder(this);
            var matchersBuilder = new MatchersBuilder(this);

            foreach (var property in model.Properties)
            {
                implementationBuilder.AddProperty(property);
                receivedBuilder.AddProperty(property);
                configurerBuilder.AddProperty(property);
                matchersBuilder.AddProperty(property);
            }

            foreach (var eventModel in model.Events)
            {
                var enrichedModel = new EnrichedEventModel(eventModel);

                implementationBuilder.AddEvent(enrichedModel);
                eventRaiserBuilder!.AddEvent(enrichedModel);
                receivedBuilder.AddEvent(enrichedModel);
            }
            
            foreach (var method in model.Methods)
            {
                var enrichedModel = new EnrichedMethodModel(method);
                
                implementationBuilder.AddMethod(enrichedModel);
                receivedBuilder.AddMethod(enrichedModel);
                configurerBuilder.AddMethod(enrichedModel);
                matchersBuilder.AddMethod(enrichedModel);
            }

            EmptyLine();
            AppendWithoutIndent(implementationBuilder.GetResult());
            EmptyLine();
            AppendWithoutIndent(receivedBuilder.GetResult());
            EmptyLine();
            AppendWithoutIndent(configurerBuilder.GetResult());
            EmptyLine();
            AppendWithoutIndent(matchersBuilder.GetResult());

            if (eventRaiserBuilder != null)
            {
                EmptyLine();
                AppendWithoutIndent(eventRaiserBuilder.GetResult());   
            }
        }
    }
}
