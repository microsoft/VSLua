using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public sealed class TrackableTextReader : IDisposable
    {
        private readonly static int HistoryLimit = 10;
        private readonly List<char> lastCharactars = new List<char>(HistoryLimit);
        private int pushedDistance = 0;
        public int Position { get; private set; }
        private readonly TextReader textReader;

        public TrackableTextReader(TextReader textReader)
        {
            Validation.Requires.NotNull(textReader, nameof(textReader));
            this.textReader = textReader;
        }

        public int Read()
        {
            if (this.Peek() != unchecked((char)-1))
            {
                char currentCharacter;
                if (this.pushedDistance == 0)
                {
                    currentCharacter = (char)textReader.Read();
                    Position++;
                    this.lastCharactars.Add(currentCharacter);
                    if (this.lastCharactars.Count > HistoryLimit)
                    {
                        this.lastCharactars.RemoveAt(0);
                    }
                    return currentCharacter;
                }
                else
                {
                    currentCharacter = this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
                    this.pushedDistance--;
                    Position++;
                    return currentCharacter;
                }

            }

            return unchecked((char)-1);
        }

        public bool EndOfStream()
        {
            return this.Peek() == unchecked((char)-1);
        }

        public char Peek()
        {
            if (this.pushedDistance == 0)
            {
                return (char)textReader.Peek();
            }
            else
            {
                Debug.Assert(this.pushedDistance <= this.lastCharactars.Count,
                    "Pushed distance greater than history count");

                return this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
            }
        }

        public char ReadChar()
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
            textReader.Dispose();
        }
    }
}

