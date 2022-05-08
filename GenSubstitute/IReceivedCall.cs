namespace GenSubstitute
{
    public interface IReceivedCall
    {
        string MethodName { get; }
        public object?[] GetArguments();
    }
}
