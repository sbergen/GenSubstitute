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
            private enum RetainState
            {
                NotConfigured,
                AlreadyConfigured,
                Retained,
            }

            private RetainState _retain = RetainState.NotConfigured;
            private T _value = default!;
            
            public ReadWrite(ConfiguredCalls calls, string? getMethodName, string? setMethodName)
                : base(calls, getMethodName, setMethodName)
            {
            }

            public ConfiguredFunc<T> Get()
            {
                ThrowIfNotConfigurable();
                return PrivateGet();
            }
            
            public ConfiguredAction<T> Set(Arg<T> arg)
            {
                ThrowIfNotConfigurable();
                return PrivateSet(arg);
            }

            /// <summary>
            /// Configures this property as a value-retaining property.
            /// If you use this option, you shouldn't configure <see cref="Get"/> or <see cref="Set"/>.
            /// </summary>
            public void RetainValue()
            {
                if (_retain == RetainState.AlreadyConfigured)
                {
                    throw new InvalidPropertyConfigurationException(
                        $"{nameof(RetainValue)} can not be called after calling {nameof(Get)} or {nameof(Set)}");
                }

                _retain = RetainState.Retained;
                PrivateGet().Configure(() => _value);
                PrivateSet(Arg<T>.Any).Configure(val => _value = val);
            }

            private void ThrowIfNotConfigurable()
            {
                if (_retain == RetainState.Retained)
                {
                    throw new InvalidPropertyConfigurationException(
                        $"{nameof(Get)} or {nameof(Set)} can not be called after calling {nameof(RetainValue)}");
                }

                _retain = RetainState.AlreadyConfigured;
            }
        }
    }
}