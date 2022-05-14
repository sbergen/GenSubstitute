using System;
using GenSubstitute.Internal;

namespace GenSubstitute
{
    public abstract class ConfiguredProperty<T>
    {
        private readonly ConfiguredCalls _calls;
        private readonly string? _getMethodName;
        private readonly string? _setMethodName;

        protected ConfiguredProperty(ConfiguredCalls calls, string? getMethodName, string? setMethodName)
        {
            _calls = calls;
            _getMethodName = getMethodName;
            _setMethodName = setMethodName;
        }

        private ConfiguredFunc<T> PrivateGet()
        {
            if (_getMethodName == null)
            {
                throw new InvalidOperationException("Get called on write-only property");
            }
            
            return _calls.Add(_getMethodName, new ConfiguredFunc<T>());
        }

        private ConfiguredAction<T> PrivateSet(Arg<T> arg)
        {
            if (_setMethodName == null)
            {
                throw new InvalidOperationException("Set called on read-only property");
            }
            
            return _calls.Add(_setMethodName, new ConfiguredAction<T>(arg));
        }

        public class ReadOnly : ConfiguredProperty<T>
        {
            public ReadOnly(ConfiguredCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public ConfiguredFunc<T> Get() => PrivateGet();
        }
        
        public class WriteOnly : ConfiguredProperty<T>
        {
            public WriteOnly(ConfiguredCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public ConfiguredAction<T> Set(Arg<T> arg) => PrivateSet(arg);
        }
        
        public class ReadWrite : ConfiguredProperty<T>
        {
            public ReadWrite(ConfiguredCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public ConfiguredFunc<T> Get() => PrivateGet();
            public ConfiguredAction<T> Set(Arg<T> arg) => PrivateSet(arg);
        }
    }
}
