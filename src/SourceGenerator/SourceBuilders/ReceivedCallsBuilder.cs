using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ReceivedCallsBuilder : ClassBuilder
    {
        public const string ClassName = "ReceivedCallsData";

        public ReceivedCallsBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ObjectSubstitutionContext)} context")
        {
            ConstructorLine("_context = context;");
            
            Line($"private readonly {nameof(ObjectSubstitutionContext)} _context;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly ReceivedPropertyCalls<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_context, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
        }
        
        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
            Line($"public IReceivedCallsInfo<{method.ReceivedCallType}> {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                
                Line($"_context.Received.GetMatching<{method.ReceivedCallType}>(");
                using (Indent())
                {
                    Line($"new {method.MatcherType}({method.MatcherConstructorArgs}));");
                }
            }
        }

        public void AddEvent(EnrichedEventModel eventModel)
        {
            EmptyLine();
            Line($"public readonly {nameof(ReceivedEventCalls<object>)}<{eventModel.Type}> {eventModel.Name};");
            ConstructorLine($"{eventModel.Name} = new(_context, \"{eventModel.AddMethodName}\", \"{eventModel.RemoveMethodName}\");");
        }
    }
}
