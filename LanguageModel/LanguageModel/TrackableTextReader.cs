using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LanguageService
{
    public sealed class TrackableTextReader : IDisposable
    {
        private readonly static int HistoryLimit = 10;
        private readonly List<char> lastCharacters = new List<char>(HistoryLimit);
        private int pushedDistance = 0;
        public int Position { get; private set; }
        private readonly TextReader textReader;
        private const char EOF = unchecked((char)-1);

        public TrackableTextReader(TextReader textReader)
        {
            Validation.Requires.NotNull(textReader, nameof(textReader));
            this.textReader = textReader;
        }

        public int Read()
        {
            if (this.Peek() == EOF)
            {
                return EOF;
            }

            char currentCharacter;
            if (this.pushedDistance == 0)
            {
                int readChar = (char)textReader.Read();
                Debug.Assert((readChar & 0xFFFF0000) == 0, "The top 16 bits returned by Read were non-zero, they will be lost when cast to char");

                currentCharacter = (char)readChar;
                Position++;
                this.lastCharacters.Add(currentCharacter);
                if (this.lastCharacters.Count > HistoryLimit)
                {
                    this.lastCharacters.RemoveAt(0);
                }
                return currentCharacter;
            }
            else
            {
                currentCharacter = this.lastCharacters[this.lastCharacters.Count - this.pushedDistance];
                this.pushedDistance--;
                Position++;

                return currentCharacter;
            }

        }

        internal bool EndOfStream()
        {
            return this.Peek() == EOF;
        }

        internal char Peek()
        {
            if (this.pushedDistance == 0)
            {
                return (char)textReader.Peek();
            }
            else
            {
                Debug.Assert(this.pushedDistance <= this.lastCharacters.Count,
                    "Pushed distance greater than history count");

                return this.lastCharacters[this.lastCharacters.Count - this.pushedDistance];
            }
        }

        internal char ReadChar()
        {
            return (char)this.Read();
        }

        public void Pushback(int amount = 1)
        {
            if (amount >= 0 && this.pushedDistance + amount <= HistoryLimit && amount <= this.Position)
            {
                this.Position -= amount;
                this.pushedDistance += amount;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void Dispose()
        {
            textReader?.Dispose();
        }
    }
}

