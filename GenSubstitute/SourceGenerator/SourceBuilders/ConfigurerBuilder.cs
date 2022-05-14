using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : ClassBuilder
    {
        public const string ClassName = "Configurer";

        public ConfigurerBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ConfiguredCalls)} configuredCalls")
        {
            ConstructorLine("_calls = configuredCalls;");
            
            Line($"private readonly {nameof(ConfiguredCalls)} _calls;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly ConfiguredProperty<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_calls, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
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
