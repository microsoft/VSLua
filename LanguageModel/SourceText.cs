using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    public class SourceText
    {
        public readonly TextReader textReader;
        public SourceText(TextReader textReader)
        {
            this.textReader = textReader;
        }
    }
}
