using System.IO;

namespace LanguageService
{
    public class SourceText
    {
        public SourceText(TextReader textReader)
        {
            Validation.Requires.NotNull(textReader, nameof(textReader));

            this.TextReader = textReader;
        }
        public TextReader TextReader { get; }
    }
}
