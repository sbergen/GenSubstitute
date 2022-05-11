using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ReceivedCallsBuilder : ClassBuilder
    {
        public const string ClassName = "ReceivedCallsData";

        public ReceivedCallsBuilder(SourceBuilder parent)
            : base(parent, $"public class {ClassName}")
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
        }

        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
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
