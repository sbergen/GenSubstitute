using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GenSubstitute
{
    public static class Arg
    {
        public static readonly AnyArg Any = new();

        public static Arg<T> Matches<T>(
            Func<T, bool> matcher,
            [CallerArgumentExpression("matcher")] string? expression = null) => new(matcher, expression);
        
        public static Arg<T> Is<T>(
            T value,
            [CallerArgumentExpression("value")] string? expression = null) => new(value, expression);
    }
    
    public readonly struct Arg<T>
    {
        private readonly Func<T, bool> _matches;
        private readonly string _expression;
        
        public static readonly Arg<T> Default =
            new(val => EqualityComparer<T>.Default.Equals(val, default!));

        public Arg(
            Func<T, bool> matcher,
            [CallerArgumentExpression("matcher")] string? expression = null)
        {
            _matches = matcher;
            _expression = expression ?? $"expression on {typeof(T)}";
        }

        public Arg(
            T value,
            [CallerArgumentExpression("value")] string? expression = null)
        {
            _matches = val => EqualityComparer<T>.Default.Equals(val, value);
            _expression = expression ?? value?.ToString() ?? $"({typeof(T)})null";
        }

        public static readonly Arg<T> Any = new (_ => true, $"any {typeof(T)}");
        
        public static implicit operator Arg<T>(AnyArg _) => Any;
        
        // ReSharper disable once RedundantArgumentDefaultValue
        public static implicit operator Arg<T>(T value) => new(value, null);

        public bool Matches(T val) => _matches(val);

        public override string ToString() => _expression;
    }
}
