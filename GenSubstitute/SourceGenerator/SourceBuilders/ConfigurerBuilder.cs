using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : ClassBuilder
    {
        public const string ClassName = "Configurer";

        public ConfigurerBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ISubstitutionContext)} context")
        {
            ConstructorLine("_context = context;");
            
            Line($"private readonly {nameof(ISubstitutionContext)} _context;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly ConfiguredProperty<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_context.Configured, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
        }

        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
            Line($"public {method.ConfiguredCallType} {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line("_context.Configured.Add(");
                using (Indent())
                {
                    Line($"{method.ResolvedMethodName},");
                    Line($"new {method.ConfiguredCallType}({method.SafeParameterNames}));");
                }
            }
        }
    }
}
