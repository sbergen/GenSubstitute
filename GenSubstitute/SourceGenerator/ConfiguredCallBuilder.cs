using System.Linq;

namespace GenSubstitute.SourceGenerator
{
    internal static class ConfiguredCallBuilder
    {
        public static string Make(MockMethodInfo method)
        {
            var callType = method.ReturnsVoid ? nameof(ConfiguredAction) : nameof(ConfiguredFunc<int>);
            var parameterArguments = method.Parameters.Select(p => p.Type);
            var allArguments = method.ReturnsVoid
                ? parameterArguments.ToList()
                : parameterArguments.Append(method.ReturnType).ToList();

            return allArguments.Any()
                ? $"{callType}<{string.Join(", ", allArguments)}>"
                : callType;
        }
    }
}
