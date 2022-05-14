using System.Linq;
using GenSubstitute.Internal;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ImplementationBuilder : ClassBuilder
    {
        public const string ClassName = "Implementation";

        public ImplementationBuilder(SourceBuilder parent, TypeModel model)
            : base(parent, $"private class {ClassName} : {model.FullyQualifiedName}")
        {
            Line($"private readonly {nameof(ConfiguredCalls)} _configuredCalls;");
            Line($"private readonly {nameof(ReceivedCalls)} _receivedCalls;");
            EmptyLine();
            Line($"internal {ClassName}({nameof(ReceivedCalls)} receivedCalls, {nameof(ConfiguredCalls)} configuredCalls)");
            Line("{");
            using (Indent())
            {
                Line("_receivedCalls = receivedCalls;");
                Line("_configuredCalls = configuredCalls;");
            }
            Line("}");
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
                    Line($"get => PropertyImplementation<{property.Type}>.Get(_receivedCalls, _configuredCalls, \"{getMethodName}\");");
                }
                
                if (property.SetMethodName is { } setMethodName)
                {
                    Line($"set => PropertyImplementation<{property.Type}>.Set(_receivedCalls, _configuredCalls, \"{setMethodName}\", value);");
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

            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"{method.ResolvedMethodName}, typeof({method.ReturnType})"
                : $"{method.ResolvedMethodName}, typeof({method.ReturnType}), {receivedCallParameterArguments}";
            
            EmptyLine();
            Line($"public {method.ReturnType} {method.Name}{method.GenericNames}({parametersWithTypes})");
            Line("{");
            using (Indent())
            {
                Line($"var receivedCall = new {method.ReceivedCallType}({receivedCallConstructorArgs});");
                Line("_receivedCalls.Add(receivedCall);");
                Line($"var call = _configuredCalls.Get<{method.ConfiguredCallType}>({method.ResolvedMethodName}, receivedCall);");
                
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
    }
}
