using GenSubstitute.SourceGenerator.Models;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class EventRaiserBuilder : ClassBuilder
    {
        public const string ClassName = "EventRaiser";
        
        public EventRaiserBuilder(SourceBuilder parent)
            : base(
                parent,
                ClassName,
                $"{ImplementationBuilder.ClassName} implementation")
        {
            ConstructorLine("_implementation = implementation;");

            Line($"private readonly {ImplementationBuilder.ClassName} _implementation;");
        }

        public void AddEvent(EnrichedEventModel eventModel)
        {
            EmptyLine();
            Line($"public void {eventModel.Name}({eventModel.InvokeParameters}) =>");
            using (Indent())
            {
                Line($"_implementation.{eventModel.InvokeMethodName}({eventModel.InvokeArguments});");
            }
        }
    }
}
