using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute.Internal
{
    /// <summary>
    /// Keeps track of received calls, only intended for internal use
    /// </summary>
    public class ReceivedCalls
    {
        private readonly List<IReceivedCall> _calls = new();

        public IReadOnlyList<IReceivedCall> All => _calls;
        public void Add(IReceivedCall call) => _calls.Add(call);
        
        public IReceivedCallsInfo<T> GetMatching<T>(ICallMatcher matcher)
            where T : IReceivedCall
            => new ReceivedCallsInfo<T>(_calls, matcher);

        public IEnumerable<IReceivedCall> ForSubstitute(ISubstitute substitute) => _calls
            .Where(c => c.Substitute == substitute);

    }
}
