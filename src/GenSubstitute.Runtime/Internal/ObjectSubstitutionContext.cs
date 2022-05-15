namespace GenSubstitute.Internal
{
    public class ObjectSubstitutionContext : ISubstitutionContext
    {
        private readonly ISubstitutionContext _context;

        public readonly ISubstitute Substitute;
        public ReceivedCalls Received => _context.Received;
        public ConfiguredCalls Configured => _context.Configured;
        
        public ObjectSubstitutionContext(ISubstitute substitute, ISubstitutionContext context)
        {
            Substitute = substitute;
            _context = context;
        }
    }
}
