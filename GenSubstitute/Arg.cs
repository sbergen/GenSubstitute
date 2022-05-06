using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    public abstract class Arg
    {
        internal abstract bool Matches(TypeValuePair val);
    }
    
    public class Arg<T> : Arg
    {
        private readonly Func<T, bool> _matches;
        private Arg(Func<T, bool> matcher) => _matches = matcher;

        public static readonly Arg<T> Default =
            new(val => EqualityComparer<T>.Default.Equals(val, default));
        
        public static readonly Arg<T> Any = new (_ => true);

        public static Arg<T> When(Func<T, bool> matcher) => new(matcher);

        public static implicit operator Arg<T>(T configuredValue) =>
            new(val => EqualityComparer<T>.Default.Equals(val, configuredValue));

        internal override bool Matches(TypeValuePair val) =>
            val.Type == typeof(T) && _matches((T)val.Value);
    }
}
