using System.Linq;
using static GenSubstitute.SourceGenerator.Utilities.ListStringUtils;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct EnrichedEventModel
    {
        public readonly string Name;
        public readonly string Type;
        public readonly string AddMethodName;
        public readonly string RemoveMethodName;
        public readonly string InvokeMethodName;
        
        // E.g. int i, double d...
        public readonly string InvokeParameters;
        
        // E.g. i, d...
        public readonly string InvokeArguments;

        public EnrichedEventModel(EventModel model)
        {
            Name = model.Name;
            Type = model.Type;
            AddMethodName = model.AddMethodName;
            RemoveMethodName = model.RemoveMethodName;
            InvokeMethodName = model.InvokeMethod.Name;
            
            InvokeParameters = BuildList(
                model.InvokeMethod.Parameters.Select(p => $"{p.Type} {p.Name}"));
            InvokeArguments = BuildList(
                model.InvokeMethod.Parameters.Select(p => p.Name));
        }
    }
}
