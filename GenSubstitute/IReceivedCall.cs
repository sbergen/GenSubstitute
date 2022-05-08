using System;

namespace GenSubstitute
{
    public interface IReceivedCall
    {
        Type ReturnType { get; }
        object?[] GetArguments();
    }
}
