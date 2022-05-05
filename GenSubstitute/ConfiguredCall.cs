using System;

namespace GenSubstitute
{
    public abstract class ConfiguredCall
    {
        private readonly Args _args;
        private readonly Type _returnType;

        private protected ConfiguredCall(Args args, Type returnType)
        {
            _args = args;
            _returnType = returnType;
        }

        internal bool Matches(Type returnType, TypeValuePair[] args) =>
            _returnType == returnType && _args.Matches(args);
    }
}
