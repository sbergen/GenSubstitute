using System;
using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public class AmbiguousConfiguredCallMatchException : Exception
    {
        public AmbiguousConfiguredCallMatchException(
            IReceivedCall receivedCall,
            IReadOnlyList<IConfiguredCall> matchingCalls)
            : base(BuildMessage(receivedCall, matchingCalls))
        {
        }
        
        private static string BuildMessage(
            IReceivedCall receivedCall,
            IReadOnlyList<IConfiguredCall> matchingCalls)
        {
            string FormatArguments(object?[] args) =>
                $"({string.Join(", ", args.Select(a => a?.ToString() ?? "null"))})";

            var received = FormatArguments(receivedCall.GetArguments());
            
            var matched = string.Join(
                "\n    ",
                matchingCalls.Select(m => FormatArguments(m.GetArguments())));
            
            return $@"Found multiple matches for arguments in call to '{receivedCall.MethodName}':
  Received arguments:
    {received}
  Matching configured arguments:
    {matched}";
        }
    }
}
