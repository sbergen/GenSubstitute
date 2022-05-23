using System;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct EventModel : IEquatable<EventModel>
    {
        public readonly string Name;
        public readonly string Type;
        public readonly MethodModel InvokeMethod;
        public readonly string AddMethodName;
        public readonly string RemoveMethodName;

        public EventModel(IEventSymbol eventSymbol)
        {
            Name = eventSymbol.Name;
            
            // Always make events nullable, as this is seems to be the proper way to do this.
            // We still need to call FullyQualifiedTypeNameWithNullability to get potential
            // generic type arguments to contain nullability information.
            var type = eventSymbol.Type.FullyQualifiedTypeNameWithNullability();
            Type = type.EndsWith("?") ? type : $"{type}?";
            
            // "nul ony in error cases"
            // TODO, do we need to handle the error cases?
            AddMethodName = eventSymbol.AddMethod!.Name; 
            RemoveMethodName = eventSymbol.RemoveMethod!.Name;
            
            // TODO, handle null?
            var delegateInvokeMethod = ((INamedTypeSymbol)eventSymbol.Type).DelegateInvokeMethod!;
            InvokeMethod = new(delegateInvokeMethod);
        }

        public bool Equals(EventModel other) =>
            Name == other.Name &&
            Type == other.Type &&
            InvokeMethod.Equals(other.InvokeMethod);
    }
}
