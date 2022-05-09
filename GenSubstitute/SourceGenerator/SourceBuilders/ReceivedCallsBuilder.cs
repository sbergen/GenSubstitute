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
            AppendLine($"public class {ClassName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"private readonly {nameof(ReceivedCalls)} _calls;");
                EmptyLine();
                AppendLine($"internal {ClassName}({nameof(ReceivedCalls)} calls) => _calls = calls;");
                
                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildMethod(methods[i], enrichedMethods[i]);
                }
            }
            AppendLine("}");
        }
        
        private void BuildMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            // The next line is identical to configure methods, maybe optimize?
            AppendLine($"public IReadOnlyList<{enriched.ReceivedCallType}> {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_calls.GetMatching<{enriched.ReceivedCallType}>(");
                using (Indent())
                {
                    AppendLine($"{enriched.ResolvedMethodName},");
                    AppendLine($"new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
                }
                
            }
        }
    }
}
