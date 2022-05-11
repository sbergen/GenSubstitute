using System.Collections.Immutable;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : SourceBuilder.Nested
    {
        public const string ClassName = "Configurer";

        public static void Build(
            SourceBuilder parent,
            ImmutableArray<EnrichedMethodModel> methods)
        {
            new ConfigurerBuilder(parent).BuildContent(methods);
        }

        private ConfigurerBuilder(SourceBuilder parent)
            : base(parent)
        {
        }

        private void BuildContent(ImmutableArray<EnrichedMethodModel> methods)
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
            Line($"public {method.ConfiguredCallType} {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line("_configuredCalls.Add(");
                using (Indent())
                {
                    Line($"{method.ResolvedMethodName},");
                    Line($"new {method.ConfiguredCallType}({method.SafeParameterNames}));");
                }
            }
        }
    }
}
