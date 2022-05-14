using System;

namespace GenSubstitute.Internal
{
    public class PropertyMatcher<T>
    {
        private readonly ISubstitute _substitute;
        private readonly Lazy<FuncMatcher<T>> _getMatcher;
        private readonly string? _setMethodName;

        public PropertyMatcher(ISubstitute substitute, string? getMethodName, string? setMethodName)
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
        
        public FuncMatcher<T> Get() => _getMatcher.Value;

        public ActionMatcher<T> Set(Arg<T> arg)
        {
            if (_setMethodName == null)
            {
                throw new InvalidOperationException("Set called on read-only property");
            }
            
            return new(_substitute, _setMethodName, arg);
        }
    }
}
