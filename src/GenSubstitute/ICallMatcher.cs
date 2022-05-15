namespace GenSubstitute
{
    public interface ICallMatcher
    {
        ISubstitute Substitute { get; }
        string MethodName { get; }
        public bool Matches(IReceivedCall call);
        public object?[] GetArguments();
    }
}
