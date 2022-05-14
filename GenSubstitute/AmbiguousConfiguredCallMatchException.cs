using System;
using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public class AmbiguousConfiguredCallMatchException : Exception
    {
        public AmbiguousConfiguredCallMatchException(
            string methodName,
            object?[] receivedArguments,
            IReadOnlyList<IConfiguredCall> matchingCalls)
            : base(BuildMessage(methodName, receivedArguments, matchingCalls))
        {
        }
        
        private static string BuildMessage(
            string methodName,
            object?[] receivedArguments,
            IReadOnlyList<IConfiguredCall> matchingCalls)
        {
            string FormatArguments(object?[] args) =>
                string.Join(", ", args.Select(a => a?.ToString() ?? "null"));

            var received = FormatArguments(receivedArguments);
            
            var matched = string.Join(
                "\n  ",
                matchingCalls.Select(m => FormatArguments(m.GetArguments())));
            
            return $@"Found multiple matches for arguments in call to '{methodName}':
  {received}
matching configured arguments:
  {matched}";
        }
    }
}
