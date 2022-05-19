using System.Linq;
using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;
using GenSubstitute.SourceGenerator.Utilities;
using static GenSubstitute.SourceGenerator.Utilities.ListStringUtils;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ImplementationBuilder : ClassBuilder
    {
        public const string ClassName = "Implementation";

        public ImplementationBuilder(SourceBuilder parent, TypeModel model)
            : base(
                parent,
                ClassName,
                $"{nameof(ObjectSubstitutionContext)} context",
                $" : {model.FullyQualifiedName}")
                // This was private for a while, but it's kind of hard with events.
                // Will see if it's really necessary, not trivial with source generators...
        {
            ConstructorLine("_context = context;");

            Line($"private readonly {nameof(ObjectSubstitutionContext)} _context;");
        }

        public void AddProperty(PropertyModel property)
        {
            EmptyLine();
            Line($"public {property.Type} {property.Name}");
            Line("{");
            using (Indent())
            {
                if (property.GetMethodName is { } getMethodName)
                {
                    Line($"get => PropertyImplementation<{property.Type}>.Get(_context, \"{getMethodName}\");");
                }
                
                if (property.SetMethodName is { } setMethodName)
                {
                    Line($"set => PropertyImplementation<{property.Type}>.Set(_context, \"{setMethodName}\", value);");
                }
            }
            Line("}");
        }
        
        public void AddMethod(EnrichedMethodModel method)
        {
            var parametersWithTypes = BuildList(
                method.Parameters
                    .Select(p => $"{p.RefKindString}{p.Type} {p.Name}"));

            var receivedCallParameterArguments = BuildList(method.Parameters
                .Select(p => p.IsOut ? $"Out<{p.Type}>.Default" : p.Name));
            
            var returnTypeForTypeof = method.ReturnType.TrimEnd('?'); // This is a bit ugly, but works
            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"{method.ResolvedMethodName}, typeof({returnTypeForTypeof})"
                : $"{method.ResolvedMethodName}, typeof({returnTypeForTypeof}), {receivedCallParameterArguments}";
            
            EmptyLine();
            Line($"public {method.ReturnType} {method.Name}{method.GenericNames}({parametersWithTypes})");
            Line("{");
            using (Indent())
            {
                Line($"var receivedCall = new {method.ReceivedCallType}(_context.Substitute, {receivedCallConstructorArgs});");
                Line("_context.Received.Add(receivedCall);");
                Line($"var call = _context.Configured.Get<{method.ConfiguredCallType}>(receivedCall);");
                
                foreach (var parameter in method.RefOrOutParameters)
                {
                    Line(parameter.LocalVariableDeclaration);
                }

                Line(method.ReturnsVoid
                    ? $"call?.Execute({method.ConfiguredCallArguments});"
                    : $"var result = call != null ? call.Execute({method.ConfiguredCallArguments}) : default!;");
                
                foreach (var parameter in method.RefOrOutParameters)
                {
                    Line(parameter.ResultAssignment);
                }

                if (!method.ReturnsVoid)
                {
                    Line("return result;");
                }
            }
            Line("}");
        }

        public void AddEvent(EnrichedEventModel eventModel)
        {
            EmptyLine();

            var privateEventName = InternalName.Make(eventModel.Name);

            string MakeReceivedCalls(string methodName) =>
                $"new {eventModel.ReceivedCallType}(_context.Substitute, \"{methodName}\", typeof(void), value)";
            
            Line($"private event {eventModel.Type} {privateEventName};");
            Line($"public event {eventModel.Type} {eventModel.Name}");
            Line("{");
            using (Indent())
            {
                Line("add");
                Line("{");
                using (Indent())
                {
                    Line($"{privateEventName} += value;");
                    Line($"var receivedCall = {MakeReceivedCalls(eventModel.AddMethodName)};");
                    Line("_context.Received.Add(receivedCall);");
                }
                Line("}");
                Line("remove");
                Line("{");
                using (Indent())
                {
                    Line($"{privateEventName} -= value;");
                    Line($"var receivedCall = {MakeReceivedCalls(eventModel.RemoveMethodName)};");
                    Line("_context.Received.Add(receivedCall);");
                }
                Line("}");
            }
            Line("}");
            
            EmptyLine();
            Line($"public void {eventModel.InvokeMethodName}({eventModel.InvokeParameters})");
            Line("{");
            using (Indent())
            {
                Line($"{privateEventName}?.Invoke({eventModel.InvokeArguments});");
            }
            Line("}");

        }
    }
}
