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
            ReceivedCalls receivedCalls,
            ConfiguredCalls configuredCalls,
            string getMethodName)
        {
            var receivedCall = new ReceivedCall(getMethodName, typeof(int));
            receivedCalls.Add(receivedCall);
            var call = configuredCalls.Get<ConfiguredFunc<T>>(getMethodName, receivedCall);
            return call != null ? call.Execute() : default!;
        }

        public static void Set(
            ReceivedCalls receivedCalls,
            ConfiguredCalls configuredCalls,
            string setMethodName,
            T value)
        {
            var receivedCall = new ReceivedCall<T>(setMethodName, typeof(void), value);
            receivedCalls.Add(receivedCall);
            var call = configuredCalls.Get<ConfiguredAction<T>>(setMethodName, receivedCall);
            call?.Execute(value);
        }
    }
}
