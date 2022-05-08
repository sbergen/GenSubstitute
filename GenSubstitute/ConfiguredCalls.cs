using System;
using System.Collections.Generic;
using System.Linq;
using GenSubstitute.Utilities;

namespace GenSubstitute
{
    /// <summary>
    /// Keeps track of configured calls, only intended for internal use
    /// (TODO, move to Internal namespace?)
    /// </summary>
    public class ConfiguredCalls
    {
        private readonly Dictionary<string, List<IConfiguredCall>> _calls = new();
    
        public T Add<T>(string methodName, T call)
            where T : class, IConfiguredCall
        {
            _calls.AddToList(methodName, call);
            return call;
        }

        public T? Get<T>(string methodName, IReceivedCall receivedCall)
            where T : class, IConfiguredCall
        {
            if (_calls.TryGetValue(methodName, out var values))
            {
                var matches = values
                    .Where(c => c.Matches(receivedCall))
                    .ToList();
            
                return matches.Count switch
                {
                    0 => null,
                    1 => (T)matches[0],
                    _ => throw new Exception("Multiple matches"),
                };
            }
            else
            {
                return null;  
            }
        }
    }
}
