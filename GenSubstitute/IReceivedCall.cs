namespace GenSubstitute
{
    public interface IReceivedCall
    {
        object Substitute { get; }
        string MethodName { get; }
        public object?[] GetArguments();
    }
}
