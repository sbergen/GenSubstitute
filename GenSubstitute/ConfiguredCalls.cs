using System;
using System.Collections.Generic;
using System.Linq;

namespace GenSubstitute
{
    public class ConfiguredCalls
    {
        private readonly Dictionary<string, List<ConfiguredCall>> _calls = new();
    
        public T Add<T>(string methodName, T call)
            where T : ConfiguredCall
        {
            if (_calls.TryGetValue(methodName, out var values))
            {
                values.Add(call);
            }
            else
            {
                values = new List<ConfiguredCall> { call };
                _calls.Add(methodName!, values);
            }

            return call;
        }

        public T Get<T>(string methodName, Type returnType, TypeValuePair[] args)
            where T : ConfiguredCall
        {
            if (_calls.TryGetValue(methodName, out var values))
            {
                var matches = values
                    .Where(c => c.Matches(returnType, args))
                    .ToList();
            
                return matches.Count switch
                {
                    0 => null,
                    1 => (T)matches[0],
                    _ => throw new Exception("Multiple matches"),
                };
            }
            else
            {
                return null;  
            }
        }
    }
}
