using System;

namespace GenSubstitute
{
    public class IncompletelyConfiguredCallException : Exception
    {
        public IncompletelyConfiguredCallException()
            : base("Method call was not fully configured")
        {
        }
    }
}
