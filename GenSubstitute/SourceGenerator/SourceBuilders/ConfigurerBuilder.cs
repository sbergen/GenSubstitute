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
            Line($"public class {ClassName}");
            Line("{");
            using (Indent())
            {
                Line($"private readonly {nameof(ConfiguredCalls)} _configuredCalls;");
                EmptyLine();
                Line($"internal {ClassName}({nameof(ConfiguredCalls)} configuredCalls)");
                Line("{");
                using (Indent())
                {
                    Line("_configuredCalls = configuredCalls;");
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
            Line($"public {enriched.ConfiguredCallType} {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                Line("_configuredCalls.Add(");
                using (Indent())
                {
                    Line($"{enriched.ResolvedMethodName},");
                    Line($"new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
                }
            }
        }
    }
}
