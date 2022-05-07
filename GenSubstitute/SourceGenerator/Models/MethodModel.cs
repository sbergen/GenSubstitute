using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using static GenSubstitute.SourceGenerator.ListStringBuilder;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct MethodModel : IEquatable<MethodModel>
    {
        public readonly bool ReturnsVoid;
        public readonly string ReturnType;
        public readonly string Name;
        public readonly ImmutableArray<ParameterModel> Parameters;
        public readonly string ConfiguredCallType;
        
        public MethodModel(IMethodSymbol symbol)
        {
            ReturnsVoid = symbol.ReturnType.SpecialType == SpecialType.System_Void;
            
            ReturnType = ReturnsVoid
                ? "void"
                : symbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var parametersBuilder = ImmutableArray.CreateBuilder<ParameterModel>();
            foreach (var parameter in symbol.Parameters)
            {
                parametersBuilder.Add(new ParameterModel(parameter));
            }

            Parameters = parametersBuilder.ToImmutable();
            Name = BuildName(symbol);
            ConfiguredCallType = MakeConfiguredCall(ReturnsVoid, ReturnType, Parameters);
        }

        public bool Equals(MethodModel other) =>
            Name == other.Name &&
            ReturnType == other.ReturnType &&
            Parameters.SequenceEqual(other.Parameters);

        private static string BuildName(IMethodSymbol symbol)
        {
            var generics = "";
            if (symbol.IsGenericMethod)
            {
                generics = $"<{BuildList(symbol.TypeParameters.Select(t => t.Name))}>";
            }
            
            return symbol.Name + generics;
        }
        
        private static string MakeConfiguredCall(
            bool returnsVoid,
            string returnType,
            ImmutableArray<ParameterModel> parameters)
        {
            if (parameters.Length == 0 && returnsVoid)
            {
                return nameof(ConfiguredAction);
            }
            else
            {
                var callType = returnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
                var parameterArguments = parameters.Select(p => p.Type);
                var allArguments = returnsVoid
                    ? parameterArguments
                    : parameterArguments.Append(returnType);

                return $"{callType}<{BuildList(allArguments)}>";
            }
        }
    }
}
