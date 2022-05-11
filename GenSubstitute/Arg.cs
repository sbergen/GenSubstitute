using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    public static class Arg
    {
        public static readonly AnyArg Any = new();
    }
    
    public readonly struct Arg<T>
    {
        private readonly Func<T, bool> _matches;
        private Arg(Func<T, bool> matcher) => _matches = matcher;

        public Arg(T value) : this(val => EqualityComparer<T>.Default.Equals(val, value))
        {}

        public static readonly Arg<T> Any = new (_ => true);
        
        public static implicit operator Arg<T>(AnyArg any) => Any;

        public static readonly Arg<T> Default =
            new(val => EqualityComparer<T>.Default.Equals(val, default!));

        public static Arg<T> When(Func<T, bool> matcher) => new(matcher);

        public static implicit operator Arg<T>(T value) => new(value);

        public bool Matches(T val) => _matches(val);
    }
}
