using System;
using System.Collections.Generic;
using GenSubstitute.Internal;

namespace GenSubstitute
{
    public abstract class ReceivedPropertyCalls<T>
    {
        private readonly ReceivedCalls _calls;
        private readonly string? _getMethodName;
        private readonly string? _setMethodName;

        protected ReceivedPropertyCalls(ReceivedCalls calls, string? getMethodName, string? setMethodName)
        {
            _calls = calls;
            _getMethodName = getMethodName;
            _setMethodName = setMethodName;
        }

        private IReadOnlyList<ReceivedCall> PrivateGet()
        {
            if (_getMethodName == null)
            {
                throw new InvalidOperationException("Get called on write-only property");
            }
            
            return _calls.GetMatching<ReceivedCall>(_getMethodName, new ConfiguredFunc<T>());
        }

        private IReadOnlyList<ReceivedCall<T>> PrivateSet(Arg<T> arg)
        {
            if (_setMethodName == null)
            {
                throw new InvalidOperationException("Set called on read-only property");
            }
            
            return _calls.GetMatching<ReceivedCall<T>>(_setMethodName, new ConfiguredAction<T>(arg));
        }

        public class ReadOnly : ReceivedPropertyCalls<T>
        {
            public ReadOnly(ReceivedCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public IReadOnlyList<ReceivedCall> Get() => PrivateGet();
        }
        
        public class WriteOnly : ReceivedPropertyCalls<T>
        {
            public WriteOnly(ReceivedCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public IReadOnlyList<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
        
        public class ReadWrite : ReceivedPropertyCalls<T>
        {
            public ReadWrite(ReceivedCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public IReadOnlyList<ReceivedCall> Get() => PrivateGet();
            public IReadOnlyList<ReceivedCall<T>> Set(Arg<T> arg) => PrivateSet(arg);
        }
    }
}
