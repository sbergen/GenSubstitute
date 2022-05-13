using System.Collections.Generic;

namespace GenSubstitute
{
    public static class ReceivedCallExtensions
    {
        public static void Times(this IReadOnlyList<IReceivedCall> calls, int times)
        {
            if (calls.Count != times)
            {
                throw ReceivedCallsAssertionException.Create($"{times} calls", calls);
            }
        }

        public static T Once<T>(this IReadOnlyList<T> calls)
            where T : IReceivedCall
        {
            if (calls.Count != 1)
            {
                throw ReceivedCallsAssertionException.Create("one call", calls);
            }

            return calls[0];
        }
    }
}
