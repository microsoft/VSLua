using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    class TrackableStreamReader : StreamReader
    {
        private int lastChar = -1;
        public TrackableStreamReader(string path) : base(path)
        {
        }

        public override int Read()
        {
            int ch;

            if (lastChar >= 0)
            {
                ch = lastChar;
                lastChar = -1;
            }
            else
            {
                ch = base.Read();  // could be -1 
            }
            return ch;
        }

        public void PushBack(char ch)  // char, don't allow Pushback(-1) TOODO: remove the pushback char...
        {
            if (lastChar >= 0)
                Console.WriteLine("ERROR: InvalidOperation PushBack of more than 1 char");

            lastChar = ch;
        }
    }
}

