using System;

namespace GenSubstitute
{
    public class AmbiguousConfiguredCallMatchException : Exception
    {
        public AmbiguousConfiguredCallMatchException()
            : base("Multiple matches")
        {
            // TODO, include more details in the message
        }
    }
}
