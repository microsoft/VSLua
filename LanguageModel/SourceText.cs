using System.IO;

namespace LanguageModel
{
    public class SourceText
    {
        public TextReader TextReader { get; }

        public SourceText(TextReader textReader)
        {
            Validation.Requires.NotNull(textReader, nameof(textReader));

            this.TextReader = textReader;
        }
    }
}
