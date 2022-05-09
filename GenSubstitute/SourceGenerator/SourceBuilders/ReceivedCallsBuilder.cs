using System.Collections.Immutable;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ReceivedCallsBuilder : SourceBuilder.Nested
    {
        public const string ClassName = "ReceivedCallsData";
        
        public static void Build(
            SourceBuilder parent,
            ImmutableArray<MethodModel> methods,
            ImmutableArray<EnrichedMethodModel> enrichedMethods)
        {
            new ReceivedCallsBuilder(parent).BuildContent(methods, enrichedMethods);
        }

        private ReceivedCallsBuilder(SourceBuilder parent)
            : base(parent)
        {
        }

        private void BuildContent(
            ImmutableArray<MethodModel> methods,
            ImmutableArray<EnrichedMethodModel> enrichedMethods)
        {
            Line($"public class {ClassName}");
            Line("{");
            using (Indent())
            {
                Line($"private readonly {nameof(ReceivedCalls)} _calls;");
                EmptyLine();
                Line($"internal {ClassName}({nameof(ReceivedCalls)} calls)");
                Line("{");
                using (Indent())
                {
                    Line("_calls = calls;");
                }
                Line("}");
                
                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildMethod(methods[i], enrichedMethods[i]);
                }
            }
            Line("}");
        }
        
        private void BuildMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            // The next line is identical to configure methods, maybe optimize?
            Line($"public IReadOnlyList<{enriched.ReceivedCallType}> {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                Line($"_calls.GetMatching<{enriched.ReceivedCallType}>(");
                using (Indent())
                {
                    Line($"{enriched.ResolvedMethodName},");
                    Line($"new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
                }
                
            }
        }
    }
}
