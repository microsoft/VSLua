using System.IO;
using Validation;

namespace LanguageService
{
    public class SourceText
    {
        public SourceText(TextReader textReader)
        {
            Requires.NotNull(textReader, nameof(textReader));

            this.TextReader = textReader;
        }

        public TextReader TextReader { get; }
    }
}
