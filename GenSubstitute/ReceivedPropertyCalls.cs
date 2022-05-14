using System;
using System.Collections.Generic;
using GenSubstitute.Internal;

namespace GenSubstitute
{
    public abstract class ReceivedPropertyCalls<T>
    {
        private readonly ObjectSubstitutionContext _context;
        private readonly string? _getMethodName;
        private readonly string? _setMethodName;

        protected ReceivedPropertyCalls(
            ObjectSubstitutionContext context,
            string? getMethodName,
            string? setMethodName)
        {
            _context = context;
            _getMethodName = getMethodName;
            _setMethodName = setMethodName;
        }

        private IReadOnlyList<ReceivedCall> PrivateGet()
        {
            if (_getMethodName == null)
            {
                throw new InvalidOperationException("Get called on write-only property");
            }
            
            return _context.Received.GetMatching<ReceivedCall>(
                new FuncMatcher<T>(_context.Substitute, _getMethodName));
        }

        private IReadOnlyList<ReceivedCall<T>> PrivateSet(Arg<T> arg)
        {
            if (_setMethodName == null)
            {
                throw new InvalidOperationException("Set called on read-only property");
            }
            
            return _context.Received.GetMatching<ReceivedCall<T>>(
                new ActionMatcher<T>(_context.Substitute, _setMethodName, arg));
        }

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
