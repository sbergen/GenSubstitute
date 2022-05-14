using System.Collections.Generic;
using GenSubstitute.Internal;

namespace GenSubstitute
{
    public abstract class ReceivedPropertyCalls<T> : PropertyMatcher<T>
    {
        private readonly ObjectSubstitutionContext _context;

        private ReceivedPropertyCalls(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
            : base(context.Substitute, getMethodName, setMethodName)
        {
            _context = context;
        }

        private IReadOnlyList<ReceivedCall> PrivateGet() =>
            _context.Received.GetMatching<ReceivedCall>(MatchGet());

        private IReadOnlyList<ReceivedCall<T>> PrivateSet(Arg<T> arg) => 
            _context.Received.GetMatching<ReceivedCall<T>>(MatchSet(arg));

        public class ReadOnly : ReceivedPropertyCalls<T>
        {
            public ReadOnly(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }
            
            public IReadOnlyList<ReceivedCall> Get() => PrivateGet();
        }
        
        public class WriteOnly : ReceivedPropertyCalls<T>
        {
            public WriteOnly(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }

            public IReadOnlyList<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
        
        public class ReadWrite : ReceivedPropertyCalls<T>
        {
            public ReadWrite(ObjectSubstitutionContext context, string? getMethodName, string? setMethodName)
                : base(context, getMethodName, setMethodName)
            {
            }

            public IReadOnlyList<ReceivedCall> Get() => PrivateGet();
            public IReadOnlyList<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
    }
}
