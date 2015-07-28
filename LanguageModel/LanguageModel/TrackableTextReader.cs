using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public class TrackableTextReader
    {
        private List<char> lastCharactars = new List<char>();
        private int historyLimit = 10;
        private int pushedDistance = 0;
        public char CurrentCharacter { get; private set; }
        public int Position { get; private set; }
        private readonly TextReader textReader;

        public TrackableTextReader(TextReader textReader, int start)
        {
            this.textReader = textReader;
            this.Position = start;
        }

        public TrackableTextReader(TextReader textReader)
        {
            this.textReader = textReader;
            // this.Position = 0;
        }

        public int Read()
        {
            // if the current character is the final character
            if (this.CurrentCharacter != unchecked((char)-1))
            {
                ++Position;
                // if the stream reader has not been pushed back at all
                if (this.pushedDistance == 0)
                {
                    this.CurrentCharacter = (char)textReader.Read();
                    this.lastCharactars.Add(this.CurrentCharacter);
                    if (this.lastCharactars.Count > this.historyLimit)
                    {
                        // limits the number of characters in the list
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
                return this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
            }
        }

        public char ReadChar()
        {
            return (char)this.Read();
        }

        public void PushBack(int n = 1)
        // if you just call PushBack(), it'll go back only one position
        // otherwise you can back up to the history limit
        {
            if (this.pushedDistance + n < this.historyLimit && n <= this.Position)
            {
                this.Position -= n;
                this.pushedDistance += n;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}

