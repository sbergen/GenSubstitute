using System;

namespace GenSubstitute
{
    internal abstract class ConfiguredCall
    {
        public readonly Args Args;
        private readonly Type _returnType;

        public ConfiguredCall(Args args, Type returnType)
        {
            Args = args;
            _returnType = returnType;
        }

        public bool Matches(Type returnType, TypeValuePair[] args) =>
            _returnType == returnType && Args.Matches(args);
    }
}
