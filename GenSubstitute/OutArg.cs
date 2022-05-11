using System;

namespace GenSubstitute
{
    public class OutArg<T> : IEquatable<OutArg<T>>
    {
        private readonly bool _isImmutable;
        private T _value = default!;

        public OutArg(bool isImmutable = true) => _isImmutable = isImmutable;

        public T Value
        {
            get => _value;
            set
            {
                if (_isImmutable)
                {
                    throw new InvalidOperationException(
                        $"You can not modify the value of an immutable {nameof(OutArg<T>)}! " +
                        $"This is most likely a value in received calls, {nameof(Default)}, or {nameof(Any)}");
                }

                _value = value;
            }
        }

        public static readonly OutArg<T> Default = new();
        public static readonly OutArg<T> Any = new();

        public static implicit operator T(OutArg<T> arg) => arg.Value;

        public static implicit operator OutArg<T>(AnyArg _) => Any;


        /// <summary>
        /// Out args are always considered equal, as they can't be read in methods before they are assigned.
        /// </summary>
        public bool Equals(OutArg<T> other) => true;
    }
}
