namespace GenSubstitute
{
    public interface ICallMatcher
    {
        public bool Matches(IReceivedCall call);
        public object?[] GetArguments();
    }
}
