using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Classification
{
    public class TagInfo
    {
        internal TagInfo(int start, int length, Classification classification)
        {
            this.Start = start;
            this.Length = length;
            this.Classification = classification;
        }

        public int Start { get; }
        public int Length { get; }
        public Classification Classification { get; }
    }
}
