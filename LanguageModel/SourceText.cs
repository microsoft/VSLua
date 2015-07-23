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
        public TextReader TextReader { get; }

        public SourceText(TextReader textReader)
        {
            Validation.Assumes.NotNull(textReader);

            this.TextReader = textReader;
        }
    }
}
