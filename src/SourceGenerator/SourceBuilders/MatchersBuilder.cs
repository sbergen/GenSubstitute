using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class MatchersBuilder : ClassBuilder
    {
        public const string ClassName = "Matchers";

        public MatchersBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ObjectSubstitutionContext)} context")
        {
            ConstructorLine("_context = context;");
            
            Line($"private readonly {nameof(ObjectSubstitutionContext)} _context;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly PropertyMatcher<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_context.Substitute, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
        }
        
        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
            Line($"public {method.MatcherType} {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line($"new({method.MatcherConstructorArgs});");
            }
        }
    }
}
