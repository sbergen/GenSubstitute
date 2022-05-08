using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    /// <summary>
    /// Keeps track of received calls, only intended for internal use
    /// (TODO, move to Internal namespace?)
    /// </summary>
    public class ReceivedCalls
    {
        private readonly List<IReceivedCall> _calls = new();

        public IReadOnlyList<IReceivedCall> All => _calls;
        public void Add(IReceivedCall call) => _calls.Add(call);

        // Note: reusing IConfiguredCall here for convenience,
        // not sure if this should use a distinct type or base type?
        public IReadOnlyList<T> GetMatching<T>(string methodName, IConfiguredCall matcher)
            where T : IReceivedCall =>
            _calls
                .Where(c => c.MethodName == methodName && matcher.Matches(c))
                .Cast<T>()
                .ToList();
    }
}
