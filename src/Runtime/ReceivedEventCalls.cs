using GenSubstitute.Internal;

namespace GenSubstitute
{
    public class ReceivedEventCalls<TDelegate>
    {
        private readonly ObjectSubstitutionContext _context;
        private readonly string _addMethodName;
        private readonly string _removeMethodName;
        
        public ReceivedEventCalls(
            ObjectSubstitutionContext context,
            string addMethodName,
            string removeMethodName)
        {
            _context = context;
            _addMethodName = addMethodName;
            _removeMethodName = removeMethodName;
        }

        public IReceivedCallsInfo<ReceivedCall<TDelegate>> Add(Arg<TDelegate> @delegate) =>
            _context.Received.GetMatching<ReceivedCall<TDelegate>>(
                new ActionMatcher<TDelegate>(_context.Substitute, _addMethodName, @delegate));

        public IReceivedCallsInfo<ReceivedCall<TDelegate>> Remove(Arg<TDelegate> @delegate) =>
            _context.Received.GetMatching<ReceivedCall<TDelegate>>(
                new ActionMatcher<TDelegate>(_context.Substitute, _removeMethodName, @delegate));
    }
}
