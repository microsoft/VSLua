using System.IO;

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
