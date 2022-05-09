using System;
using System.Collections.Immutable;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class MockBuilder : SourceBuilder
    {
        private const string ImplementationClassName = "Implementation";
        private const string ReceivedCallClassName = "ReceivedCallsData";

        public static string BuildMock(TypeModel model) => new MockBuilder(model).GetResult();

        private MockBuilder(TypeModel model)
        {
            var typeParameters = model.TypeParameters.IsEmpty ? "" : $"<{BuildList(model.TypeParameters)}>";

            AppendLine("internal static partial class GeneratorMarkerExtensions");
            AppendLine("{");
            using (Indent())
            {
                AppendLine(
                    $"public static {model.BuilderTypeName}{typeParameters} Build{typeParameters}(this GenerateMarker<{model.FullyQualifiedName}> m) => new();");
            }
            AppendLine("}");
            
            EmptyLine();
            
            AppendLine($"internal sealed class {model.BuilderTypeName}{typeParameters}");
            AppendLine("{");
            using (Indent())
            {
                BuildBuilderContents(model);
            }
            AppendLine("}");
        }

        private void BuildBuilderContents(TypeModel model)
        {
            var methods = model.Methods;
            var enrichedMethodInfo = ImmutableArray
                .CreateRange(model.Methods, m => new EnrichedMethodModel(m));

            AppendLine($"private class {ImplementationClassName} : {model.FullyQualifiedName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"public readonly {nameof(ConfiguredCalls)} ConfiguredCalls = new();");
                AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls;");
                EmptyLine();
                AppendLine($"internal {ImplementationClassName}({nameof(ReceivedCalls)} receivedCalls) => _receivedCalls = receivedCalls;");
                
                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildImplementationMethod(methods[i], enrichedMethodInfo[i]);
                }
            }
            AppendLine("}");
            
            AppendLine($"public class {ReceivedCallClassName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"private readonly {nameof(ReceivedCalls)} _calls;");
                EmptyLine();
                AppendLine($"internal {ReceivedCallClassName}({nameof(ReceivedCalls)} calls) => _calls = calls;");
                
                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildReceivedCallsMethod(methods[i], enrichedMethodInfo[i]);
                }
            }
            AppendLine("}");
            
            EmptyLine();
            AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls = new();");
            AppendLine($"private readonly {ImplementationClassName} _implementation;");
            EmptyLine();
            AppendLine($"public {model.FullyQualifiedName} Object => _implementation;");
            AppendLine($"public {ReceivedCallClassName} Received {{ get; }}");
            AppendLine($"public IReadOnlyList<{nameof(IReceivedCall)}> AllReceived => _receivedCalls.All;");
            
            AppendLine($"internal {model.BuilderTypeName}()");
            AppendLine("{");
            using (Indent())
            {
                AppendLine("_implementation = new(_receivedCalls);");
                AppendLine("Received = new(_receivedCalls);");
            }
            AppendLine("}");
            
            for (var i = 0; i < methods.Length; ++i)
            {
                EmptyLine();
                BuildConfigureMethod(methods[i], enrichedMethodInfo[i]);
            }
        }

        private void BuildImplementationMethod(MethodModel method, EnrichedMethodModel enriched)
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

        private void BuildConfigureMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            AppendLine($"public {enriched.ConfiguredCallType} {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_implementation.ConfiguredCalls.Add({enriched.ResolvedMethodName}, new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
            }
        }
        
        private void BuildReceivedCallsMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            // The next line is identical to configure methods, maybe optimize?
            AppendLine($"public IReadOnlyList<{enriched.ReceivedCallType}> {method.Name}{enriched.GenericNames}({enriched.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_calls.GetMatching<{enriched.ReceivedCallType}>({enriched.ResolvedMethodName}, new {enriched.ConfiguredCallType}({enriched.SafeParameterNames}));");
            }
        }
    }
}
