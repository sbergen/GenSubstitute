using System;
using System.Collections.Generic;
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

                for (var i = 0; i < model.Methods.Length; ++i)
                {
                    EmptyLine();
                    BuildMethod(model.Methods[i], enrichedMethods[i]);
                }
            }
            Line("}");
        }
        
        private void BuildMethod(MethodModel method, EnrichedMethodModel enriched)
        {
            var parameters = method.Parameters;
            
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
            
            Line($"public {method.ReturnType} {method.Name}{enriched.GenericNames}({parametersWithTypes})");
            Line("{");
            using (Indent())
            {
                Line($"var receivedCall = new {enriched.ReceivedCallType}({receivedCallConstructorArgs});");
                Line("_receivedCalls.Add(receivedCall);");
                Line($"var call = _configuredCalls.Get<{enriched.ConfiguredCallType}>({enriched.ResolvedMethodName}, receivedCall);");

                EmptyLine();
                var callParameterArray = new string[parameters.Length];
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    if (parameter.RefKind == RefKind.Ref)
                    {
                        // TODO, uniqueness not guaranteed here!
                        var name = $"{parameter.Name}_RefArg";
                        Line($"var {name} = new RefArg<{parameter.Type}>({parameter.Name});");
                        callParameterArray[i] = name;
                    }
                    else
                    {
                        callParameterArray[i] = parameter.Name;
                    }
                }
                EmptyLine();

                var callParametersStr = BuildList(callParameterArray);
                Line(method.ReturnsVoid
                    ? $"call?.Execute({callParametersStr});"
                    : $"var result = call != null ? call.Execute({callParametersStr}) : default!;");

                // TODO, this is really damn ugly right now!
                EmptyLine();
                foreach (var parameter in parameters)
                {
                    if (parameter.RefKind == RefKind.Ref)
                    {
                        // TODO, uniqueness not guaranteed here!
                        var name = $"{parameter.Name}_RefArg";
                        Line($"{parameter.Name} = {name};");
                    }
                }
                EmptyLine();
                
                if (!method.ReturnsVoid)
                {
                    Line("return result;");
                }
            }
            Line("}");
        }
    }
}
