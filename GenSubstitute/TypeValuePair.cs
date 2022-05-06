using System;

namespace GenSubstitute
{
    public readonly struct TypeValuePair
    {
        public readonly Type Type;
        public readonly object Value;

        private TypeValuePair(Type type, object value)
        {
            Type = type;
            Value = value;
        }

        public static TypeValuePair Make<T>(T val) => new (typeof(T), val);
    }
}
