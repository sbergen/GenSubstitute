using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public class ReceivedCallsInfo<T> : IReceivedCallsInfo<T>
        where T : IReceivedCall
    {
        public IReadOnlyList<IReceivedCall> All { get; }
        public IReadOnlyList<T> Matching { get; }
        public ICallMatcher Matcher { get; }

        public ReceivedCallsInfo(IEnumerable<IReceivedCall> all, ICallMatcher matcher)
        {
            Matcher = matcher;
            All = all.ToList();
            Matching = All
                .Where(matcher.Matches)
                .Cast<T>()
                .ToList();
        }

        int IReadOnlyCollection<T>.Count => Matching.Count;
        T IReadOnlyList<T>.this[int index] => Matching[index];
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Matching.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Matching).GetEnumerator();
    }
}
