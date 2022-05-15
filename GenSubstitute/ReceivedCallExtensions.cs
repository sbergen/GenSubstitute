using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public static class ReceivedCallExtensions
    {
        public static void Times(this IReadOnlyList<IReceivedCall> calls, int times)
        {
            if (calls.Count != times)
            {
                throw ReceivedCallsAssertionException.Create($"{times} calls,", calls);
            }
        }

        public static T Once<T>(this IReadOnlyList<T> calls)
            where T : IReceivedCall
        {
            if (calls.Count != 1)
            {
                throw ReceivedCallsAssertionException.Create("one call,", calls);
            }

            return calls[0];
        }

        public static void InOrder(this IEnumerable<IReceivedCall> calls, params ICallMatcher[] matchers)
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

            var matchersStr = string.Join("\n\t", matchers.AsEnumerable());
            var expectedStr = $"calls matching in order:\n\t{matchersStr}\n";
            throw ReceivedCallsAssertionException.Create(expectedStr, allCalls);
        }
    }
}
