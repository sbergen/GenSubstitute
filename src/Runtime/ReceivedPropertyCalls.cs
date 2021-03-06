using GenSubstitute.Internal;

namespace GenSubstitute
{
    public abstract class ReceivedPropertyCalls<T>
    {
        private readonly ObjectSubstitutionContext _context;
        private readonly PropertyMatcher<T> _matcher;

        private ReceivedPropertyCalls(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
        {
            _context = context;
            _matcher = new(context.Substitute, getMethodName, setMethodName);
        }

        private IReceivedCallsInfo<ReceivedCall> PrivateGet() =>
            _context.Received.GetMatching<ReceivedCall>(_matcher.MatchGet());

        private IReceivedCallsInfo<ReceivedCall<T>> PrivateSet(Arg<T> arg) => 
            _context.Received.GetMatching<ReceivedCall<T>>(_matcher.MatchSet(arg));

        public class ReadOnly : ReceivedPropertyCalls<T>
        {
            public ReadOnly(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }
            
            public IReceivedCallsInfo<ReceivedCall> Get() => PrivateGet();
        }
        
        public class WriteOnly : ReceivedPropertyCalls<T>
        {
            public WriteOnly(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }

            public IReceivedCallsInfo<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
        
        public class ReadWrite : ReceivedPropertyCalls<T>
        {
            public ReadWrite(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }

            public IReceivedCallsInfo<ReceivedCall> Get() => PrivateGet();
            public IReceivedCallsInfo<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
    }
}
