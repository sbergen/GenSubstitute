using System;

namespace GenSubstitute
{
    public class InvalidPropertyConfigurationException : Exception
    {
        public InvalidPropertyConfigurationException(string message)
            : base(message)
        {
        }
    }
}
