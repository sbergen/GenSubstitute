using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace GenSubstitute.SourceGenerator.Utilities
{
    /// <summary>
    /// SourceText implementation optimized for append-only indented lines.
    /// </summary>
    internal class GenSourceText : SourceText
    {
        private readonly List<Line> _lines = new(128); // Assume around at least this many lines
        private int _length;

        public void AddLine(int indent, string content)
        {
            var line = new Line(_length, indent, content);
            _length += line.Length;
            _lines.Add(line);
        }

        public void EmptyLine() => AddLine(0, "");

        public void Add(GenSourceText other)
        {
            foreach (var line in other._lines)
            {
                var newLine = new Line(_length, line);
                _length += newLine.Length;
                _lines.Add(newLine);
            }
        }

        public override Encoding Encoding => Encoding.UTF8;
        public override int Length => _length;

        public override char this[int position] => CharAt(position);
        
        public override void CopyTo(
            int sourceIndex,
            char[] destination,
            int destinationIndex,
            int count)
        {
            // TODO add argument validation
            
            // Skip lines
            var lineIndex = 0;
            while (!_lines[lineIndex].ContainsPosition(sourceIndex))
            {
                ++lineIndex;
            }
            
            // Copy lines
            var toCopy = count;
            while (toCopy > 0)
            {
                var line = _lines[lineIndex];
                var offsetInLine = sourceIndex - line.StartIndex;
                var countForLine = Math.Min(line.Length - offsetInLine, toCopy);
                
                line.CopyTo(
                    sourceIndex,
                    destination,
                    destinationIndex,
                    countForLine);

                toCopy -= countForLine;
                sourceIndex += countForLine;
                destinationIndex += countForLine;
                ++lineIndex;
            }
        }

        private char CharAt(int position)
        {
            foreach (var line in _lines)
            {
                if (line.ContainsPosition(position))
                {
                    return line.CharAt(position);
                }
            }

            throw new ArgumentOutOfRangeException(
                nameof(position),
                position,
                "Position should be less than length of the text.");
        }
        
        private readonly struct Line
        {
            private const char IndentChar = '\t';
            private const char NewlineChar = '\n';
            
            private readonly string _content;
            private readonly int _indentAmount;

            public readonly int StartIndex;
            public readonly int Length;

            public Line(int startIndex, int indentAmount, string content)
            {
                StartIndex = startIndex;
                Length = indentAmount + content.Length + 1;
                _indentAmount = indentAmount;
                _content = content;
            }

            // Moves line to have another position
            public Line(int startIndex, Line other)
            {
                StartIndex = startIndex;
                Length = other.Length;
                _indentAmount = other._indentAmount;
                _content = other._content;
            }

            public bool ContainsPosition(int position) =>
                position >= StartIndex &&
                position < StartIndex + Length;

            public char CharAt(int sourceIndex)
            {
                var offsetInContent = sourceIndex - StartIndex - _indentAmount;
                if (offsetInContent < 0)
                {
                    return IndentChar;
                }
                else if (offsetInContent < _content.Length)
                {
                    return _content[offsetInContent];
                }
                else
                {
                    return NewlineChar;
                }
            }

            public void CopyTo(
                int sourceIndex,
                char[] destination,
                int destinationIndex,
                int count)
            {
                var indentToAdd = Math.Min(count, StartIndex + _indentAmount - sourceIndex);
                while (indentToAdd > 0)
                {
                    destination[destinationIndex++] = IndentChar;
                    --indentToAdd;
                    --count;
                    ++sourceIndex;
                }

                var contentIndex = sourceIndex - StartIndex - _indentAmount;
                var fromContent = Math.Min(count, _content.Length - contentIndex);

                if (fromContent > 0)
                {
                    _content.CopyTo(
                        contentIndex,
                        destination,
                        destinationIndex,
                        fromContent);
                    count -= fromContent;
                    destinationIndex += fromContent;
                }

                if (count > 0)
                {
                    destination[destinationIndex] = NewlineChar;
                }
            }
        }
    }
}
