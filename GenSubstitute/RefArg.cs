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
        private readonly bool _isImmutable;
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (_isImmutable)
                {
                    throw new InvalidOperationException(
                        $"You can not modify the value of an immutable {nameof(RefArg<T>)}! " +
                        $"This is most likely a value in received calls, {nameof(Default)}, or {nameof(Any)}");
                }

                _value = value;
            }
        }

        public RefArg(T value, bool isImmutable = true) => (_value, _isImmutable) = (value, isImmutable);

        public static readonly RefArg<T> Default = new(default!);
        public static readonly RefArg<T> Any = new(default!);

        public static implicit operator RefArg<T>(T val) => new (val);
        public static implicit operator RefArg<T>(AnyArg unused) => Any;
        public static implicit operator T(RefArg<T> val) => val.Value;

        public bool Equals(RefArg<T> other) =>
            this == Any ||
            other == Any ||
            EqualityComparer<T>.Default.Equals(Value, other.Value);
    }
}
