namespace GenSubstitute.Internal
{
    /// <summary>
    /// Property call implementation helper. This is a simplified case of method calls,
    /// so it can be statically implemented instead of fully generated.
    /// </summary>
    /// <remarks>
    /// Might later be extended to support retaining property values!
    /// </remarks>
    public static class PropertyImplementation<T>
    {
        public static T Get(
            ISubstitutionContext context,
            string getMethodName)
        {
            var receivedCall = new ReceivedCall(getMethodName, typeof(int));
            context.Received.Add(receivedCall);
            var call = context.Configured.Get<ConfiguredFunc<T>>(getMethodName, receivedCall);
            return call != null ? call.Execute() : default!;
        }

        public static void Set(
            ISubstitutionContext context,
            string setMethodName,
            T value)
        {
            var receivedCall = new ReceivedCall<T>(setMethodName, typeof(void), value);
            context.Received.Add(receivedCall);
            var call = context.Configured.Get<ConfiguredAction<T>>(setMethodName, receivedCall);
            call?.Execute(value);
        }
    }
}
