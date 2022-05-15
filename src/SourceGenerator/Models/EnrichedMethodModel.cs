using System.Collections.Immutable;
using System.Linq;
using static GenSubstitute.SourceGenerator.Utilities.ListStringUtils;

namespace GenSubstitute.SourceGenerator.Models
{
    /// <summary>
    /// Contains data derived from <see cref="MethodModel"/>,
    /// which is not required for caching, but is useful in source generation. 
    /// </summary>
    internal readonly struct EnrichedMethodModel
    {
        // TODO: This is a bit more tightly coupled than I would like it to be :(
        private const string MatcherExpression = "_context.Substitute";
        
        public readonly string Name;
        public readonly string ReturnType;
        public readonly bool ReturnsVoid;

        public readonly ImmutableArray<EnrichedParameterModel> Parameters;
        public readonly ImmutableArray<RefOrOutParameterModel> RefOrOutParameters;

        // E.g. <T1, T2>
        public readonly string GenericNames;
        
        // E.g. $"SomeMethod<{typeof(T1).FullName}, {typeof(T2).FullName}>
        public readonly string ResolvedMethodName;
        
        // E.g. "Arg<int>? intArg, Arg<double>? doubleArg"
        public readonly string ArgParameters;

        // E.g. "_context.Substitute, "SomeMethod", intArg ?? Arg<int>.Default, doubleArg ?? Arg<double>.Default"
        public readonly string MatcherConstructorArgs;

        // E.g. ConfiguredFunc<int, double>
        public readonly string ConfiguredCallType;
        
        // E.g. FuncMatcher<int, double>
        public readonly string MatcherType;
        
        // E.g. ReceivedCall<int>
        public readonly string ReceivedCallType;
        
        // E.g. normalArg, byRefArg_local
        public readonly string ConfiguredCallArguments;
        
        public EnrichedMethodModel(MethodModel method)
        {
            Name = method.Name;
            ReturnType = method.ReturnType;
            ReturnsVoid = method.ReturnsVoid;

            Parameters = ImmutableArray.CreateRange(
                method.Parameters,
                p => new EnrichedParameterModel(p));
            
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

            ArgParameters = BuildList(Parameters
                .Select(p => $"{p.WrappedType}? {p.Name}{p.DefaultValueDeclaration}"));

            MatcherConstructorArgs = BuildList(Parameters
                .Select(p => $"{p.Name} ?? {p.DefaultValueConstructor}")
                .Prepend(ResolvedMethodName)
                .Prepend(MatcherExpression));

            var callObjectParameterList = BuildList(Parameters.Select(p => p.CallObjectType));
            if (method.Parameters.Length == 0 && method.ReturnsVoid)
            {
                ConfiguredCallType = nameof(ConfiguredAction);
                MatcherType = nameof(ActionMatcher);
            }
            else
            {
                var callType = method.ReturnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
                var matcherType = method.ReturnsVoid ? nameof(ActionMatcher) : nameof(FuncMatcher<int>);
                var returnType = (method.ReturnsVoid, method.Parameters.Length) switch
                {
                    (true, _) => "",
                    (false, 0) => method.ReturnType,
                    _ => $", {method.ReturnType}",
                };

                ConfiguredCallType = $"{callType}<{callObjectParameterList}{returnType}>";
                MatcherType = $"{matcherType}<{callObjectParameterList}{returnType}>";
            }

            ReceivedCallType = method.Parameters.Length == 0
                ? "ReceivedCall"
                : $"ReceivedCall<{callObjectParameterList}>";
            
            var refOrOutBuilder = ImmutableArray.CreateBuilder<RefOrOutParameterModel>();
            var callArgumentsBuilder = ImmutableArray.CreateBuilder<string>(Parameters.Length);
            foreach (var parameter in Parameters)
            {
                if (parameter.IsRef || parameter.IsOut)
                {
                    var refOrOutModel = new RefOrOutParameterModel(parameter);
                    refOrOutBuilder.Add(refOrOutModel);
                    callArgumentsBuilder.Add(refOrOutModel.LocalVariableName);
                }
                else
                {
                    callArgumentsBuilder.Add(parameter.Name);
                }
            }

            RefOrOutParameters = refOrOutBuilder.ToImmutable();
            ConfiguredCallArguments = BuildList(callArgumentsBuilder);
        }
    }
}
