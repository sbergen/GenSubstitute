using System.Collections.Immutable;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ReceivedCallsBuilder : SourceBuilder.Nested
    {
        public const string ClassName = "ReceivedCallsData";
        
        public static void Build(
            SourceBuilder parent,
            ImmutableArray<EnrichedMethodModel> methods)
        {
            new ReceivedCallsBuilder(parent).BuildContent(methods);
        }

        private ReceivedCallsBuilder(SourceBuilder parent)
            : base(parent)
        {
        }

        private void BuildContent(ImmutableArray<EnrichedMethodModel> methods)
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

                foreach (var method in methods)
                {
                    EmptyLine();
                    BuildMethod(method);
                }
            }
            Line("}");
        }
        
        private void BuildMethod(EnrichedMethodModel method)
        {
            // The next line is identical to configure methods, maybe optimize?
            Line($"public IReadOnlyList<{method.ReceivedCallType}> {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line($"_calls.GetMatching<{method.ReceivedCallType}>(");
                using (Indent())
                {
                    Line($"{method.ResolvedMethodName},");
                    Line($"new {method.ConfiguredCallType}({method.SafeParameterNames}));");
                }
                
            }
        }
    }
}
