using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : ClassBuilder
    {
        public const string ClassName = "Configurer";

        public ConfigurerBuilder(SourceBuilder parent)
            : base(parent, $"public class {ClassName}")
        {
            Line($"private readonly {nameof(ConfiguredCalls)} _calls;");
            EmptyLine();
            Line($"internal {ClassName}({nameof(ConfiguredCalls)} configuredCalls)");
            Line("{");
            using (Indent())
            {
                Line("_calls = configuredCalls;");
            }
            Line("}");
        }

        public void AddProperty(PropertyModel property)
        {
            // TODO: move to constructor to use a single instance
            EmptyLine();
            Line($"public ConfiguredProperty<{property.Type}>.{property.HelperSubType} {property.Name} =>");
            using (Indent())
            {
                Line($"new(_calls, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
            }
        }

        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
            Line($"public {method.ConfiguredCallType} {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line("_calls.Add(");
                using (Indent())
                {
                    Line($"{method.ResolvedMethodName},");
                    Line($"new {method.ConfiguredCallType}({method.SafeParameterNames}));");
                }
            }
        }
    }
}
