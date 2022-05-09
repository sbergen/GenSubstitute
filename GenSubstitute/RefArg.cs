using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    /// <summary>
    /// Represents an argument passed by reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RefArg<T> : IEquatable<RefArg<T>>
    {
        public T Value { get; set; }

        public RefArg(T value) => Value = value;
        
        public static implicit operator RefArg<T>(T val) => new (val);
        public static implicit operator T(RefArg<T> val) => val.Value;

        public static readonly RefArg<T> Default = new(default!);

        public bool Equals(RefArg<T> other) => EqualityComparer<T>.Default.Equals(Value, other.Value);
    }
}
