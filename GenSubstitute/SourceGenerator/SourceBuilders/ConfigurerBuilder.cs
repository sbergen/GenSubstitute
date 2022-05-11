using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : ClassBuilder
    {
        public const string ClassName = "Configurer";

        public ConfigurerBuilder(SourceBuilder parent)
            : base(parent, $"public class {ClassName}")
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
        }

        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
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
