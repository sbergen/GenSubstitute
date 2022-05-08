using System.Collections.Generic;
using System.Linq;
using GenSubstitute.SourceGenerator.Models;
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
            // Cache reused info (could preallocate and iterate methods only once to maybe optimize?)
            var methods = model.Methods;
            var configuredCalls = methods.Select(MakeConfiguredCall).ToList();
            var generics = methods.Select(m => new GenericSpecifiers(m)).ToList();
            var argLists = methods.Select(m => new ArgLists(m)).ToList();
            var receivedCallTypes = methods
                .Select(m => m.Parameters.Length == 0
                    ? "ReceivedCall"
                    : $"ReceivedCall<{BuildList(m.Parameters.Select(p => p.Type))}>")
                .ToList();
            
            AppendLine($"private class {ImplementationClassName} : {model.FullyQualifiedName}");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"public readonly {nameof(ConfiguredCalls)} ConfiguredCalls = new();");
                AppendLine($"public readonly {nameof(ReceivedCalls)} _receivedCalls;");
                EmptyLine();
                AppendLine($"internal {ImplementationClassName}({nameof(ReceivedCalls)} receivedCalls) => _receivedCalls = receivedCalls;");
                
                for (var i = 0; i < methods.Length; ++i)
                {
                    EmptyLine();
                    BuildImplementationMethod(methods[i], configuredCalls[i], receivedCallTypes[i], generics[i]);
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
                    BuildReceivedCallsMethod(methods[i], configuredCalls[i], receivedCallTypes[i], generics[i], argLists[i]);
                }
            }
            AppendLine("}");
            
            EmptyLine();
            AppendLine($"private readonly {nameof(ReceivedCalls)} _receivedCalls = new();");
            AppendLine($"private readonly {ImplementationClassName} _implementation;");
            EmptyLine();
            AppendLine($"public {model.FullyQualifiedName} Object => _implementation;");
            AppendLine($"public {ReceivedCallClassName} Received {{ get; }}");
            
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
                BuildConfigureMethod(methods[i], configuredCalls[i], generics[i], argLists[i]);
            }
        }

        private void BuildImplementationMethod(
            MethodModel method,
            string configuredCallType,
            string receivedCallType,
            GenericSpecifiers generics)
        {
            var parametersWithTypes = BuildList(
                method.Parameters
                .Select(p => $"{p.Type} {p.Name}"));

            var parameterNames = BuildList(method.Parameters.Select(p => p.Name));

            var receivedCallConstructorArgs = method.Parameters.Length == 0
                ? $"{generics.ResolvedMethodName}, typeof({method.ReturnType})"
                : $"{generics.ResolvedMethodName}, typeof({method.ReturnType}), {parameterNames}";
            
            AppendLine($"public {method.ReturnType} {method.Name}{generics.GenericNames}({parametersWithTypes})");
            AppendLine("{");
            using (Indent())
            {
                AppendLine($"var receivedCall = new {receivedCallType}({receivedCallConstructorArgs});");
                AppendLine("_receivedCalls.Add(receivedCall);");
                AppendLine($"var call = ConfiguredCalls.Get<{configuredCallType}>({generics.ResolvedMethodName}, receivedCall);");

                AppendLine(method.ReturnsVoid
                    ? $"call?.Execute({parameterNames});"
                    : $"return call != null ? call.Execute({parameterNames}) : default!;");
            }
            AppendLine("}");
        }

        private void BuildConfigureMethod(
            MethodModel method,
            string configuredCallType,
            GenericSpecifiers generics,
            ArgLists argLists)
        {
            AppendLine($"public {configuredCallType} {method.Name}{generics.GenericNames}({argLists.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_implementation.ConfiguredCalls.Add({generics.ResolvedMethodName}, new {configuredCallType}({argLists.SafeParameterNames}));");
            }
        }
        
        private void BuildReceivedCallsMethod(
            MethodModel method,
            string configuredCallType,
            string receivedCallType,
            GenericSpecifiers generics,
            ArgLists argLists)
        {
            // The next line is identical to configure methods, maybe optimize?
            AppendLine($"public {nameof(IReadOnlyList<bool>)}<{receivedCallType}> {method.Name}{generics.GenericNames}({argLists.ArgParameters}) =>");
            using (Indent())
            {
                AppendLine($"_calls.GetMatching<{receivedCallType}>({generics.ResolvedMethodName}, new {configuredCallType}({argLists.SafeParameterNames}));");
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
        
        // Caches string used in multiple places
        private readonly struct GenericSpecifiers
        {
            // E.g. <T1, T2>
            public readonly string GenericNames;
            
            // E.g. $"SomeMethod<{typeof(T1).FullName}, {typeof(T2).FullName}>
            public readonly string ResolvedMethodName;

            public GenericSpecifiers(MethodModel method)
            {
                if (method.GenericParameterNames.Length == 0)
                {
                    GenericNames = "";
                    ResolvedMethodName = $"\"{method.Name}\"";
                }
                else
                {
                    GenericNames = $"<{BuildList(method.GenericParameterNames)}>";
                    
                    var fullNames = method.GenericParameterNames
                        .Select(p => $"{{typeof({p}).FullName}}");
                    ResolvedMethodName = $"$\"{method.Name}<{BuildList(fullNames)}>\"";
                }
            }
        }

        // Caches string used in multiple places
        private readonly struct ArgLists
        {
            // E.g. "Arg<int>? intArg, Arg<double>? doubleArg"
            public readonly string ArgParameters;

            // E.g. "intArg ?? Arg<int>.Default, doubleArg ?? Arg<double>.Default"
            public readonly string SafeParameterNames;

            public ArgLists(MethodModel method)
            {
                ArgParameters = BuildList(method.Parameters
                    .Select(p => $"Arg<{p.Type}>? {p.Name}"));
                
                SafeParameterNames = BuildList(method.Parameters
                    .Select(p => $"{p.Name} ?? Arg<{p.Type}>.Default"));
            }
        }
    }
}
