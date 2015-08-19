using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Text
{
    public class SourceTextLine
    {
        internal SourceTextLine(string text, int start, int length)
        {
            this.Text = text;
            this.Start = start;
            this.Length = length;
            this.End = start + length;
        }

        public string Text { get; }

        public int Start { get; }

        public int Length { get; }

        public int End { get; }

        public TextReader TextReader => new StringReader(this.Text);
    }
}
