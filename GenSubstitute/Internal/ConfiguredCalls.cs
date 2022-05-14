using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute.Internal
{
    /// <summary>
    /// Keeps track of configured calls, only intended for internal use
    /// </summary>
    public class ConfiguredCalls
    {
        private readonly List<IConfiguredCall> _calls = new();

        public T Add<T>(T call)
            where T : class, IConfiguredCall
        {
            _calls.Add(call);
            return call;
        }

        public T? Get<T>(string methodName, IReceivedCall receivedCall)
            where T : class, IConfiguredCall
        {
            var matches = _calls
                .Where(c => c.Matches(receivedCall))
                .ToList();

            return matches.Count switch
            {
                0 => null,
                1 => (T)matches[0],
                _ => throw new AmbiguousConfiguredCallMatchException(methodName, receivedCall.GetArguments(), matches),
            };
        }
    }
}
