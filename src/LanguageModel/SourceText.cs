/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using LanguageService.Text;
using Validation;

namespace LanguageService
{
    public class SourceText
    {
        public SourceText(string text)
        {
            Requires.NotNull(text, nameof(text));
            this.text = text;
            this.Lines = this.GetLines();
        }

        private string text;

        public ImmutableArray<SourceTextLine> Lines { get; }

        public TextReader TextReader => new StringReader(this.text);

        public int? GetLineNumberFromIndex(int index)
        {
            for (int i = 0; i < this.Lines.Length; ++i)
            {
                if (index >= this.Lines[i].Start && index < this.Lines[i].End)
                {
                    return i;
                }
            }

            return null;
        }

        private ImmutableArray<SourceTextLine> GetLines()
        {
            List<SourceTextLine> lines = new List<SourceTextLine>();

            StringBuilder currentLine = new StringBuilder();

            int start = 0;

            bool addLine = false;

            for (int i = 0; i < this.text.Length; ++i)
            {
                if (addLine)
                {
                    addLine = false;
                    lines.Add(new SourceTextLine(currentLine.ToString(), start, i - 1 - start));
                    currentLine.Clear();
                    start = i - 1;
                }

                currentLine.Append(this.text[i]);

                if (this.text[i] == '\r')
                {
                    addLine = true;

                    if (i < this.text.Length - 1 || this.text[i + 1] == '\n')
                    {
                        i++;
                        currentLine.Append(this.text[i]);
                        continue;
                    }

                    continue;
                }

                if (this.text[i] == '\n')
                {
                    addLine = true;
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(new SourceTextLine(currentLine.ToString(), start, this.text.Length - start));
            }

            return lines.ToImmutableArray();
        }
    }
}
