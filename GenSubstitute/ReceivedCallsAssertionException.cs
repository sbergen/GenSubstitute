using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    public class ReceivedCallsAssertionException : Exception
    {
        public static ReceivedCallsAssertionException Create<T>(
            string message,
            IReadOnlyList<T> calls)
            where T : IReceivedCall
            => new(BuildFullMessage(message, calls));

        private ReceivedCallsAssertionException(string message) : base(message)
        {
        }
        
        private static string BuildFullMessage<T>(
            string message,
            IReadOnlyList<T> calls)
            where T : IReceivedCall
            => $"Expected to receive {message}, got:\n\t{string.Join("\n\t", calls)}";
    }
}
