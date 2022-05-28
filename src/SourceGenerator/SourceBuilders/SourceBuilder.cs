using System;
using GenSubstitute.SourceGenerator.Utilities;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class SourceBuilder
    {
        private GenSourceText _builder = new();
        private int _indentation;

        protected abstract void FinalizeContent();

        protected SourceBuilder()
        {
        }
        
        /// <summary>
        /// Constructor for nested builders, which will inherit the indentation level 
        /// </summary>
        protected SourceBuilder(SourceBuilder parent)
        {
            _indentation = parent._indentation;
        }
        
        public GenSourceText GetResult()
        {
            if (_builder == null)
            {
                throw new InvalidOperationException($"{GetType().Name} result already used!");
            }

            FinalizeContent();
            var result = _builder;
            _builder = null!;
            return result;
        }

        protected void Line(string line) =>
            _builder.AddLine(_indentation, line);

        protected void Consume(SourceBuilder other) =>
            _builder.Add(other.GetResult());

        protected void EmptyLine() => _builder.EmptyLine();

        protected IndentationScope Indent() => new(this);

        internal readonly struct IndentationScope : IDisposable
        {
            private readonly SourceBuilder _parent;

            public IndentationScope(SourceBuilder parent)
            {
                _parent = parent;
                ++_parent._indentation;
            }

            public void Dispose()
            {
                --_parent._indentation;
            }
        }
    }
}
