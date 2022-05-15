namespace GenSubstitute.Internal
{
    public interface ISubstitutionContext
    {
        ReceivedCalls Received { get; }
        ConfiguredCalls Configured { get; }
    }
}
