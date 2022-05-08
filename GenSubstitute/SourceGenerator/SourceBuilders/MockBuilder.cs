using System.Linq;
using GenSubstitute.SourceGenerator.Models;
using static GenSubstitute.SourceGenerator.Utilities.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal class MockBuilder : SourceBuilder
    {
        private const string ImplementationClassName = "Implementation";

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
            var configuredCalls = methods.Select(MakeConfiguredCall).ToList();
            
            AppendLine($"private class {ImplementationClassName} : {model.FullyQualifiedName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine("public readonly ConfiguredCalls Calls = new();");

                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildImplementationMethod(methods[i], configuredCalls[i]);
                }
            }
            AppendLine("}");
            
            EmptyLine();
            AppendLine($"private readonly {ImplementationClassName} _implementation = new();");
            EmptyLine();
            AppendLine($"public {model.FullyQualifiedName} Object => _implementation;");
            
            for (var i = 0; i < methods.Length; ++i)
            {
                EmptyLine();
                BuildConfigureMethod(methods[i], configuredCalls[i]);
            }
        }

        private void BuildImplementationMethod(MethodModel method, string configuredCallType)
        {
            var parametersWithTypes = BuildList(
                method.Parameters
                .Select(p => $"{p.Type} {p.Name}"));

            var parameterNames = BuildList(method.Parameters.Select(p => p.Name));
            var receivedCallType = method.Parameters.Length == 0
                ? "ReceivedCall"
                : $"ReceivedCall<{BuildList(method.Parameters.Select(p => p.Type))}>";

            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"typeof({method.ReturnType})"
                : $"typeof({method.ReturnType}), {parameterNames}";
            
            AppendLine($"public {method.ReturnType} {method.Name}({parametersWithTypes})");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"var receivedCall = new {receivedCallType}({receivedCallConstructorArgs});");
                AppendLine($"var call = Calls.Get<{configuredCallType}>(\"{method.Name}\", receivedCall);");

                AppendLine(method.ReturnsVoid
                    ? $"call?.Execute({parameterNames});"
                    : $"return call != null ? call.Execute({parameterNames}) : default!;");
            }
            AppendLine("}");
        }

        private void BuildConfigureMethod(MethodModel method, string configuredCallType)
        {
            var argParameters = BuildList(method.Parameters
                .Select(p => $"Arg<{p.Type}>? {p.Name}"));

            var parameterNames = BuildList(method.Parameters
                .Select(p => $"{p.Name} ?? Arg<{p.Type}>.Default"));
            
            AppendLine($"public {configuredCallType} {method.Name}({argParameters}) =>");
            using (Indent())
            {
                AppendLine($"_implementation.Calls.Add(\"{method.Name}\", new {configuredCallType}({parameterNames}));");
            }
        }

        private static string MakeConfiguredCall(MethodModel method)
        {
            if (method.Parameters.Length == 0 && method.ReturnsVoid)
            {
                return nameof(ConfiguredAction);
            }
            else
            {
                var callType = method.ReturnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
                var parameterArguments = method.Parameters.Select(p => p.Type);
                var allArguments = method.ReturnsVoid
                    ? parameterArguments
                    : parameterArguments.Append(method.ReturnType);

                return $"{callType}<{BuildList(allArguments)}>";
            }
        }
    }
}
