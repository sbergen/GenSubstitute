namespace GenSubstitute
{
    public interface IReceivedCall
    {
        ISubstitute Substitute { get; }
        string MethodName { get; }
        public object?[] GetArguments();
    }
}
