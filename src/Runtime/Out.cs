using System;

namespace GenSubstitute
{
    public class Out<T> : IEquatable<Out<T>>
    {
        private readonly bool _isImmutable;
        private T _value = default!;

        /// <summary>
        /// Intended only for internal use.
        /// </summary>
        public static Out<T> CreateForExecutionContext() => new(isImmutable: false);
        
        private Out(bool isImmutable = true) => _isImmutable = isImmutable;

        public T Value
        {
            get => _value;
            set
            {
                if (_isImmutable)
                {
                    throw new InvalidOperationException(
                        $"You can not modify the value of an immutable {nameof(Out<T>)}! " +
                        $"This is most likely a value in received calls or {nameof(Default)}.");
                }

                _value = value;
            }
        }

        public static readonly Out<T> Default = new();

        public static implicit operator T(Out<T> arg) => arg.Value;

        /// <summary>
        /// Out args are always considered equal, as they can't be read in methods before they are assigned.
        /// </summary>
        public bool Equals(Out<T> other) => true;
    }
}
