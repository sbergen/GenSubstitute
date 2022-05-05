namespace GenSubstitute
{
    internal class Args
    {
        private readonly Arg[] _args;
        
        public Args(params Arg[] args) => _args = args;

        public bool Matches(TypeValuePair[] args)
        {
            if (args.Length != _args.Length)
            {
                return false;
            }

            for (int i = 0; i < _args.Length; i++)
            {
                if (!_args[i].Matches(args[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
