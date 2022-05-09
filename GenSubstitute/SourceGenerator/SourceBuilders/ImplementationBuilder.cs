using System;
using System.Collections.Immutable;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
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
            AppendLine($"private class {ClassName} : {model.FullyQualifiedName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"public readonly {nameof(ConfiguredCalls)} ConfiguredCalls = new();");
                AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls;");
                EmptyLine();
                AppendLine($"internal {ClassName}({nameof(ReceivedCalls)} receivedCalls) =>");
                using (Indent())
                {
                    AppendLine("_receivedCalls = receivedCalls;");
                }

                for (var i = 0; i < model.Methods.Length; ++i)
                {
                    EmptyLine();
                    BuildMethod(model.Methods[i], enrichedMethods[i]);
                }
            }
            AppendLine("}");
        }
        
        private void BuildMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            // TODO: Is there a better way to do this?
            static string RefString(ParameterModel p) => p.RefKind switch
            {
                RefKind.None => "",
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var parametersWithTypes = BuildList(
                method.Parameters
                    .Select(p => $"{RefString(p)}{p.Type} {p.Name}"));

            var parameterNames = BuildList(method.Parameters.Select(p => p.Name));

            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"{enriched.ResolvedMethodName}, typeof({method.ReturnType})"
                : $"{enriched.ResolvedMethodName}, typeof({method.ReturnType}), {parameterNames}";
            
            AppendLine($"public {method.ReturnType} {method.Name}{enriched.GenericNames}({parametersWithTypes})");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"var receivedCall = new {enriched.ReceivedCallType}({receivedCallConstructorArgs});");
                AppendLine("_receivedCalls.Add(receivedCall);");
                AppendLine($"var call = ConfiguredCalls.Get<{enriched.ConfiguredCallType}>({enriched.ResolvedMethodName}, receivedCall);");

                AppendLine(method.ReturnsVoid
                    ? $"call?.Execute({parameterNames});"
                    : $"return call != null ? call.Execute({parameterNames}) : default!;");
            }
            AppendLine("}");
        }
    }
}
