using System;

namespace GenSubstitute
{
    public class OutArg<T> : IEquatable<OutArg<T>>
    {
        // TODO: I guess we shouldn't throw if the configuration does not set this?
        public T Value { get; set; } = default!;

        public static readonly OutArg<T> Default = new(); // TODO, what if this is modified?
        public static readonly OutArg<T> Any = new();

        public static implicit operator T(OutArg<T> arg) => arg.Value;

        public static implicit operator OutArg<T>(AnyArg unused) => Any;


        /// <summary>
        /// Out args are always considered equal, as they can't be read in methods before they are assigned.
        /// </summary>
        public bool Equals(OutArg<T> other) => true;
    }
}
