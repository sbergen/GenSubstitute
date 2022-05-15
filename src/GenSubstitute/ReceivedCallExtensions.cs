using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public static class ReceivedCallExtensions
    {
        public static void Times(this IReceivedCallsInfo<IReceivedCall> calls, int times)
        {
            if (calls.Matching.Count != times)
            {
                throw ReceivedCallsAssertionException.Constraint($"{times} calls", calls);
            }
        }
        
        public static void Never(this IReceivedCallsInfo<IReceivedCall> calls) => calls.Times(0);

        public static T Once<T>(this IReceivedCallsInfo<T> calls)
            where T : IReceivedCall
        {
            if (calls.Matching.Count != 1)
            {
                throw ReceivedCallsAssertionException.Constraint("one call", calls);
            }

            return calls.Matching[0];
        }

        public static void InOrder(
            this IEnumerable<IReceivedCall> calls,
            params ICallMatcher[] matchers)
        {
            var allCalls = calls.ToArray();
            var unmatched = new Queue<ICallMatcher>(matchers);

            foreach (var call in allCalls)
            {
                if (unmatched.Peek().Matches(call))
                {
                    unmatched.Dequeue();
                    if (unmatched.Count == 0)
                    {
                        return;
                    }
                }
            }
            
            throw ReceivedCallsAssertionException.InOrder(matchers, allCalls);
        }
    }
}
