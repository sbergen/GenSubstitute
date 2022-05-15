using System;
using System.Collections.Immutable;
using System.Linq;
using GenSubstitute.SourceGenerator.Utilities;
using Microsoft.CodeAnalysis;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct MethodModel : IEquatable<MethodModel>
    {
        public readonly bool ReturnsVoid;
        public readonly string ReturnType;
        public readonly string Name;
        public readonly ImmutableArray<string> GenericParameterNames;
        public readonly ImmutableArray<ParameterModel> Parameters;

        public MethodModel(IMethodSymbol symbol)
        {
            ReturnsVoid = symbol.ReturnType.SpecialType == SpecialType.System_Void;
            
            ReturnType = ReturnsVoid
                ? "void"
                : symbol.ReturnType.FullyQualifiedTypeNameWithNullability(symbol.ReceiverNullableAnnotation);

            var parametersBuilder = ImmutableArray.CreateBuilder<ParameterModel>(symbol.Parameters.Length);
            foreach (var parameter in symbol.Parameters)
            {
                parametersBuilder.Add(new ParameterModel(parameter));
            }

            Parameters = parametersBuilder.ToImmutable();
            Name = symbol.Name;
            GenericParameterNames = symbol.IsGenericMethod
                ? ImmutableArray.CreateRange(symbol.TypeParameters.Select(t => t.Name))
                : ImmutableArray<string>.Empty;
        }

        public bool Equals(MethodModel other) =>
            Name == other.Name && // property methods should have unique names
            ReturnType == other.ReturnType &&
            GenericParameterNames.SequenceEqual(other.GenericParameterNames) &&
            Parameters.SequenceEqual(other.Parameters);
    }
}
