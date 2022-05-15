using System;
using System.Collections.Generic;

namespace GenSubstitute
{
    /// <summary>
    /// Represents an argument passed by reference.
    /// </summary>
    public class Ref<T> : IEquatable<Ref<T>>
    {
        private readonly bool _isImmutable;
        private T _value;
        
        private Ref(T value, bool isImmutable = true) => (_value, _isImmutable) = (value, isImmutable);
        
        /// <summary>
        /// Intended only for internal use.
        /// </summary>
        public static Ref<T> CreateForExecutionContext(T value) => new(value, isImmutable: false);

        public T Value
        {
            get => _value;
            set
            {
                if (_isImmutable)
                {
                    throw new InvalidOperationException(
                        $"You can not modify the value of an immutable {nameof(Ref<T>)}! " +
                        "This is most likely a value in received calls");
                }

                _value = value;
            }
        }

        public static implicit operator Ref<T>(T val) => new (val);
        public static implicit operator T(Ref<T> val) => val.Value;

        public bool Equals(Ref<T> other) => EqualityComparer<T>.Default.Equals(_value, other._value);
    }
}
