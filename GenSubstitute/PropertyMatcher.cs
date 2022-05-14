using System;

namespace GenSubstitute
{
    public abstract class PropertyMatcher<T>
    {
        private readonly ISubstitute _substitute;
        private readonly Lazy<FuncMatcher<T>> _getMatcher;
        private readonly string? _setMethodName;

        protected PropertyMatcher(ISubstitute substitute, string? getMethodName, string? setMethodName)
        {
            _substitute = substitute;
            _setMethodName = setMethodName;

            _getMatcher = new(
                () =>
                {
                    if (getMethodName == null)
                    {
                        throw new InvalidOperationException("Get called on write-only property");
                    }
            
                    return new(substitute, getMethodName);
                });
        }
        
        protected FuncMatcher<T> MatchGet() => _getMatcher.Value;

        protected ActionMatcher<T> MatchSet(Arg<T> arg)
        {
            if (_setMethodName == null)
            {
                throw new InvalidOperationException("Set called on read-only property");
            }
            
            return new(_substitute, _setMethodName, arg);
        }
        
        public class ReadOnly : PropertyMatcher<T>
        {
            public ReadOnly(ISubstitute substitute, string? getMethodName, string? setMethodName)
                : base(substitute, getMethodName, setMethodName)
            {
            }
            
            public FuncMatcher<T> Get() => MatchGet();
        }
        
        public class WriteOnly : PropertyMatcher<T>
        {
            public WriteOnly(ISubstitute substitute, string? getMethodName, string? setMethodName)
                : base(substitute, getMethodName, setMethodName)
            {
            }

            public ActionMatcher<T> Set(Arg<T> arg) => MatchSet(arg);
        }
        
        public class ReadWrite : PropertyMatcher<T>
        {
            public ReadWrite(ISubstitute substitute, string? getMethodName, string? setMethodName)
                : base(substitute, getMethodName, setMethodName)
            {
            }

            public FuncMatcher<T> Get() => MatchGet();
            public ActionMatcher<T> Set(Arg<T> arg) => MatchSet(arg);
        }
    }
}
