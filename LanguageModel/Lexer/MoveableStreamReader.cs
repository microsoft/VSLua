using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageModel
{
    class MoveableStreamReader : StreamReader
    {
        private int lastChar = -1;
        public int currentPositionInStream { get; private set; } //TODO: bug regarding position in the stream?

        public MoveableStreamReader(string path) : base(path)
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
                ch = base.Read();
                if(ch != -1)
                {
                    currentPositionInStream++;
                }
            }
            return ch;
        }

        public void PushBack(char ch)  // char, don't allow Pushback(-1)
        {
            if (lastChar >= 0)
                Console.WriteLine("ERROR: InvalidOperation PushBack of more than 1 char");

            currentPositionInStream--;
            lastChar = ch;
        }
    }
}
