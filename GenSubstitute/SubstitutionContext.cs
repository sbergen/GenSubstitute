using GenSubstitute.Internal;

namespace GenSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        private readonly ReceivedCalls _receivedCalls = new();
        private readonly ConfiguredCalls _configuredCalls = new();

        ReceivedCalls ISubstitutionContext.Received => _receivedCalls;
        ConfiguredCalls ISubstitutionContext.Configured => _configuredCalls;
    }
}
