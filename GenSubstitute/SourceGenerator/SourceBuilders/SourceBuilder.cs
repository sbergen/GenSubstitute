using System;
using System.Text;

namespace GenSubstitute.SourceGenerator.SourceBuilders
{
    internal abstract class SourceBuilder
    {
        private StringBuilder _builder = new();
        private int _indentation;

        protected SourceBuilder()
        {
            AppendLine("#nullable enable");
            EmptyLine();
            AppendLine("using System.Collections.Generic;");
            EmptyLine();
            AppendLine("namespace GenSubstitute");
            AppendLine("{");
            ++_indentation;
        }

        public string GetResult()
        {
            if (_builder == null)
            {
                throw new InvalidOperationException($"{GetType().Name} result already used!");
            }
            
            --_indentation;
            AppendLine("}");
            
            var result = _builder.ToString();
            _builder = null!;
            return result;
        }

        protected void AppendLine(string line)
        {
            _builder.Append('\t', this._indentation);
            _builder.AppendLine(line);
        }

        protected void EmptyLine() => _builder.AppendLine();

        protected IndentationScope Indent() => new(this);

        // This exists just for shorter syntax, as this stuff is used a lot.
        public abstract class Nested
        {
            private readonly SourceBuilder _parent;

            protected Nested(SourceBuilder parent) => _parent = parent;

            protected void AppendLine(string line) => _parent.AppendLine(line);
            protected void EmptyLine() => _parent.EmptyLine();
            protected IndentationScope Indent() => new(_parent);
        }
        
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
