using System.Collections.Immutable;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class ImplementationBuilder : SourceBuilder.Nested
    {
        public const string ClassName = "Implementation";

        public static void Build(
            SourceBuilder parent,
            TypeModel model,
            ImmutableArray<EnrichedMethodModel> enrichedMethods)
        {
            new ImplementationBuilder(parent).BuildContent(model, enrichedMethods);
        }

        private ImplementationBuilder(SourceBuilder parent)
            : base(parent)
        {
        }

        private void BuildContent(
            TypeModel model,
            ImmutableArray<EnrichedMethodModel> enrichedMethods)
        {
            Line($"private class {ClassName} : {model.FullyQualifiedName}");
            Line("{");
            using (Indent())
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

                foreach (var method in enrichedMethods)
                {
                    EmptyLine();
                    BuildMethod(method);   
                }
            }
            Line("}");
        }
        
        private void BuildMethod(EnrichedMethodModel method)
        {
            var parametersWithTypes = BuildList(
                method.Parameters
                    .Select(p => $"{p.RefKindString}{p.Type} {p.Name}"));

            var parameterNames = BuildList(method.Parameters.Select(p => p.Name));

            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"{method.ResolvedMethodName}, typeof({method.ReturnType})"
                : $"{method.ResolvedMethodName}, typeof({method.ReturnType}), {parameterNames}";
            
            Line($"public {method.ReturnType} {method.Name}{method.GenericNames}({parametersWithTypes})");
            Line("{");
            using (Indent())
            {
                Line($"var receivedCall = new {method.ReceivedCallType}({receivedCallConstructorArgs});");
                Line("_receivedCalls.Add(receivedCall);");
                Line($"var call = _configuredCalls.Get<{method.ConfiguredCallType}>({method.ResolvedMethodName}, receivedCall);");

                EmptyLine();
                foreach (var parameter in method.RefOrOutParameters)
                {
                    Line(parameter.LocalVariableDeclaration);
                }

                if (method.RefOrOutParameters.Any())
                {
                    EmptyLine();
                }
                
                Line(method.ReturnsVoid
                    ? $"call?.Execute({method.ConfiguredCallArguments});"
                    : $"var result = call != null ? call.Execute({method.ConfiguredCallArguments}) : default!;");
                
                EmptyLine();
                foreach (var parameter in method.RefOrOutParameters)
                {
                    Line(parameter.ResultAssignment);
                }

                if (method.RefOrOutParameters.Any())
                {
                    EmptyLine();
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
