namespace GenSubstitute
{
    public interface IConfiguredCall
    {
        public bool Matches(IReceivedCall call);
        public object?[] GetArguments();
    }
}
