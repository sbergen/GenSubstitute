using System.Collections.Immutable;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : SourceBuilder.Nested
    {
        public const string ClassName = "Configurer";

        public static void Build(
            SourceBuilder parent,
            ImmutableArray<MethodModel> methods,
            ImmutableArray<EnrichedMethodModel> enrichedMethods)
        {
            new ConfigurerBuilder(parent).BuildContent(methods, enrichedMethods);
        }

        private ConfigurerBuilder(SourceBuilder parent)
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
                AppendLine($"private readonly {nameof(ConfiguredCalls)} _configuredCalls;");
                EmptyLine();
                AppendLine($"internal {ClassName}({nameof(ConfiguredCalls)} configuredCalls)");
                AppendLine("{");
                using (Indent())
                {
                    AppendLine("_configuredCalls = configuredCalls;");
                }
                AppendLine("}");
                
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
            AppendLine($"public {enriched.ConfiguredCallType} {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine("_configuredCalls.Add(");
                using (Indent())
                {
                    AppendLine($"{enriched.ResolvedMethodName},");
                    AppendLine($"new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
                }
            }
        }
    }
}
