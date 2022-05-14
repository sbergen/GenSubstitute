using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringUtils;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ConfigurerBuilder : ClassBuilder
    {
        public const string ClassName = "Configurer";

        public ConfigurerBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ObjectSubstitutionContext)} context")
        {
            ConstructorLine("_context = context;");
            
            Line($"private readonly {nameof(ObjectSubstitutionContext)} _context;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly ConfiguredProperty<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_context, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
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
                    var allParameters = PrependToListString("_context.Substitute", method.SafeParameterNames);
                    Line($"new {method.ConfiguredCallType}(new({allParameters})));");
                }
            }
        }
    }
}
