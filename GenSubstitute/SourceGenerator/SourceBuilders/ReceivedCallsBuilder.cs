using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ReceivedCallsBuilder : ClassBuilder
    {
        public const string ClassName = "ReceivedCallsData";

        public ReceivedCallsBuilder(SourceBuilder parent)
            : base(parent, ClassName, $"{nameof(ReceivedCalls)} calls")
        {
            ConstructorLine("_calls = calls;");
            
            Line($"private readonly {nameof(ReceivedCalls)} _calls;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public readonly ReceivedPropertyCalls<{property.Type}>.{property.HelperSubType} {property.Name};");
            ConstructorLine($"{property.Name} = new(_calls, \"{property.GetMethodName}\", \"{property.SetMethodName}\");");
        }
        
        public void AddMethod(EnrichedMethodModel method)
        {
            EmptyLine();
            Line($"public IReadOnlyList<{method.ReceivedCallType}> {method.Name}{method.GenericNames}({method.ArgParameters}) =>");
            using (Indent())
            {
                Line($"_calls.GetMatching<{method.ReceivedCallType}>(");
                using (Indent())
                {
                    Line($"{method.ResolvedMethodName},");
                    Line($"new {method.ConfiguredCallType}({method.SafeParameterNames}));");
                }
                
            }
        }
    }
}
