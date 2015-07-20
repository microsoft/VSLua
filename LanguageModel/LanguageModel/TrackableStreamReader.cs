using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    //UNFINISHED PROTOTYPE CODE! IGNORE FOR REVIEW
    class TrackableStreamReader : StreamReader
    {
        private List<char> lastCharactars = new List<char>();
        private int historyLimit = 10;
        private int pushedDistance = 0;
        public char CurrentCharacter { get; private set; }
        public int Position { get; private set; }
        public TrackableStreamReader(string path) : base(path)
        { }

        public override int Read()
        {
            // if the current character is the final character
            if (this.CurrentCharacter != -1)
            {
                ++Position;
                // if the stream reader has not been pushed back at all
                if (this.pushedDistance == 0)
                {
                    if (this.CurrentCharacter != 0)
                    {
                        this.lastCharactars.Add(this.CurrentCharacter);
                        if (this.lastCharactars.Count > this.historyLimit)
                        {
                            // limits the number of characters in the list
                            this.lastCharactars = this.lastCharactars.GetRange(1, this.historyLimit);
                        }
                    }

                    this.CurrentCharacter = (char) base.Read();
                    return this.CurrentCharacter;
                }
                else
                {
                    this.CurrentCharacter = this.lastCharactars[this.lastCharactars.Count - this.pushedDistance];
                    ++this.pushedDistance;
                    return this.CurrentCharacter;
                }
                
            }

            return -1;
        }

        public void PushBack(int n = 1)
            // if you just call PushBack(), it'll go back only one position
            // otherwise you can back up to the history limit
        {
            if (this.pushedDistance + n > this.historyLimit || n > this.Position)
            {
                this.Position -= n;
                this.pushedDistance += n;
            }
            else
            {
                // Throw an error or something
            }
        }
    }
}

