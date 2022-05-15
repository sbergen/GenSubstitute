using System.Collections.Generic;

namespace GenSubstitute
{
    /// <summary>
    /// Interface for received calls with the related matcher (if any).
    /// This interface is mostly needed for covariance.
    /// </summary>
    /// <typeparam name="T">Concrete type of matching calls (may be IReceivedCall)</typeparam>
    public interface IReceivedCallsInfo<out T> : IReadOnlyList<T>
        where T : IReceivedCall
    {
        IReadOnlyList<IReceivedCall> All { get; }
        IReadOnlyList<T> Matching { get; }
        ICallMatcher Matcher { get; }
    }
}
