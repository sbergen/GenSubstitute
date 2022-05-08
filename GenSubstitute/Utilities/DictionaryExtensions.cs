using System.Collections.Generic;

namespace GenSubstitute.Utilities
{
    public static class DictionaryExtensions
    {
        public static List<TValue> AddToList<TKey, TValue>(
            this Dictionary<TKey, List<TValue>> dictionary,
            TKey key,
            TValue value)
        {
            if (dictionary.TryGetValue(key, out var values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<TValue> { value };
                dictionary.Add(key, values);
            }

            return values;
        }
    }
}
