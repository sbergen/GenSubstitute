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
        
        public static readonly Arg<T> Default =
            new(val => EqualityComparer<T>.Default.Equals(val, default!));
        
        public Arg(Func<T, bool> matcher) => _matches = matcher;
        public Arg(T value) => _matches = val => EqualityComparer<T>.Default.Equals(val, value);

        public static readonly Arg<T> Any = new (_ => true);
        
        public static implicit operator Arg<T>(AnyArg _) => Any;
        public static implicit operator Arg<T>(T value) => new(value);

        public bool Matches(T val) => _matches(val);
    }
}
