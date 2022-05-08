using System;
using System.Collections.Generic;
using System.Linq;
using GenSubstitute.Utilities;

namespace GenSubstitute
{
    /// <summary>
    /// Keeps track of received calls, only intended for internal use
    /// (TODO, move to Internal namespace?)
    /// </summary>
    public class ReceivedCalls
    {
        private readonly Dictionary<string, List<IReceivedCall>> _calls = new();

        public void Add(string methodName, IReceivedCall call) => _calls.AddToList(methodName, call);

        // Note: reusing IConfiguredCall here for convenience,
        // not sure if this should use a distinct type or base type?
        public IReadOnlyList<T> GetMatching<T>(string methodName, IConfiguredCall matcher)
            where T : IReceivedCall
        {
            if (_calls.TryGetValue(methodName, out var calls))
            {
                return calls
                    .Where(matcher.Matches)
                    .Cast<T>()
                    .ToList();
            }
            else
            {
                return Array.Empty<T>();
            }
        }
    }
}
