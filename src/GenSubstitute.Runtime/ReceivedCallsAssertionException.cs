using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    public class ReceivedCallsAssertionException : Exception
    {
        public static ReceivedCallsAssertionException Constraint<T>(
            string constraint,
            IReceivedCallsInfo<T> calls)
            where T : IReceivedCall
            => new(BuildConstraintMessage(constraint, calls));
        
        public static ReceivedCallsAssertionException InOrder(
            IReadOnlyList<ICallMatcher> matchers,
            IReadOnlyList<IReceivedCall> calls)
            => new(BuildInOrderMessage(matchers, calls));

        private ReceivedCallsAssertionException(string message) : base(message)
        {
        }

        private static string BuildConstraintMessage<T>(
            string constraint,
            IReceivedCallsInfo<T> calls)
            where T : IReceivedCall
        {
            return $@"Expected to receive {constraint} matching:
  {calls.Matcher}
Actually received:
  {string.Join("\n  ", calls.All)}";
        }

        private static string BuildInOrderMessage(
            IReadOnlyList<ICallMatcher> matchers,
            IReadOnlyList<IReceivedCall> calls)
        {
            return $@"Expected to receive calls in order:
  {string.Join("\n  ", matchers)}
Actually received:
  {string.Join("\n  ", calls)}";
        }
    }
}
