using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    internal class TrackableTextReader
    {
        private List<char> lastCharactars = new List<char>();
        private int historyLimit = 10;
        private int pushedDistance = 0;
        public char CurrentCharacter { get; private set; }
        public int Position { get; private set; }
        private readonly TextReader textReader;

        internal TrackableTextReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        internal int Read()
        {
            if (this.CurrentCharacter != unchecked((char)-1))
            {
                ++Position;
                if (this.pushedDistance == 0)
                {
                    this.CurrentCharacter = (char)textReader.Read();
                    this.lastCharactars.Add(this.CurrentCharacter);
                    if (this.lastCharactars.Count > this.historyLimit)
                    {
                        // limits the number of characters in the list
                        //   TODO: needs a better way to limit history, however right now it is O(1)
                        this.lastCharactars = this.lastCharactars.GetRange(1, this.historyLimit);
                    }
                    return this.CurrentCharacter;
                }
                else
                {
                    this.CurrentCharacter = this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
                    --this.pushedDistance;
                    return this.CurrentCharacter;
                }

            }

            return unchecked((char)-1);
        }

        internal bool EndOfStream()
        {
            return this.Peek() == unchecked((char)-1);
        }

        internal char Peek()
        {
            if (this.pushedDistance == 0)
            {
                return (char)textReader.Peek();
            }
            else
            {
                return this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
            }
        }

        internal char ReadChar()
        {
            return (char)this.Read();
        }

        internal void PushBack(int n = 1)
        {
            Validation.Requires.Range(n >= 0, n.ToString(), nameof(n));
            if (this.pushedDistance + n < this.historyLimit && n <= this.Position)
            {
                this.Position -= n;
                this.pushedDistance += n;
            }
            else
            {
                throw new IndexOutOfRangeException(nameof(n) + " is over history limit");
            }
        }
    }
}

