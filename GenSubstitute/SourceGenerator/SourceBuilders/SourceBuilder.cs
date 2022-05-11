using System;
using System.Text;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class SourceBuilder
    {
        private StringBuilder _builder = new();
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
        
        public string GetResult()
        {
            if (_builder == null)
            {
                throw new InvalidOperationException($"{GetType().Name} result already used!");
            }

            FinalizeContent();
            var result = _builder.ToString();
            _builder = null!;
            return result;
        }

        protected void Line(string line)
        {
            _builder.Append('\t', _indentation);
            _builder.AppendLine(line);
        }

        protected void AppendWithoutIndent(string content)
        {
            _builder.Append(content);
        }

        protected void EmptyLine() => _builder.AppendLine();

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
