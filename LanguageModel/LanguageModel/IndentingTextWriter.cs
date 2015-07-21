//Author: Andrew Arnott Andrew.Arnott@microsoft.com
//Source: index/#Microsoft.VisualStudio.Composition/IndentingTextWriter.cs,083b8cac8f07e6cd 
//File: IndentingTextWriter.cs
//Project: Microsoft.VisualStudio.Composition\Microsoft.VisualStudio.Composition.csproj(Microsoft.VisualStudio.Composition)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace LanguageService
{
    internal class IndentingTextWriter : TextWriter
    {
        private const string Indentation = "    ";
        private readonly TextWriter inner;
        private readonly Stack<string> indentationStack = new Stack<string>();

        internal IndentingTextWriter(TextWriter inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            this.inner = inner;
        }

        public override Encoding Encoding
        {
            get { return this.inner.Encoding; }
        }

        internal static IndentingTextWriter Get(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            return writer as IndentingTextWriter ?? new IndentingTextWriter(writer);
        }

        public override void WriteLine(string value)
        {
            foreach (var indent in this.indentationStack)
            {
                this.inner.Write(indent);
            }

            this.inner.WriteLine(value);
        }

        public override void Write(char value)
        {
            this.inner.Write(value);
        }

        internal CancelIndent Indent()
        {
            this.indentationStack.Push(Indentation);
            return new CancelIndent(this);
        }

        private void Unindent()
        {
            this.indentationStack.Pop();
        }

        internal struct CancelIndent : IDisposable
        {
            private readonly IndentingTextWriter writer;

            internal CancelIndent(IndentingTextWriter writer)
            {
                if (writer == null)
                {
                    throw new ArgumentNullException("writer");
                }

                this.writer = writer;
            }

            public void Dispose()
            {
                if (this.writer != null)
                {
                    this.writer.Unindent();
                }
            }
        }
    }
}
