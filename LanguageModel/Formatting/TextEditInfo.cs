using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting
{
    public class TextEditInfo
    {
        public int Start { get; private set; }
        public int Length { get; private set; }
        public string ReplacingString { get; private set; }

        internal TextEditInfo(int start, int length, string replacingString)
        {
            this.Start = start;
            this.Length = length;
            this.ReplacingString = replacingString;
        }
    }
}
