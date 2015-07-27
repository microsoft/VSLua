using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LanguageService;


namespace LanguageModel
{
    internal class ParseTreeProvider
    {
        private readonly ConditionalWeakTable<SourceText, List<Token>> sources = 
            new ConditionalWeakTable<SourceText, List<Token>>();

        internal List<Token> Get(SourceText sourceText)
        {
            Validation.Requires.NotNull(sourceText, nameof(sourceText));

            List<Token> tokens = null;
            if (sources.TryGetValue(sourceText, out tokens))
            {
                return tokens;
            }

            tokens = Lexer.Tokenize(sourceText.TextReader);
            sources.Add(sourceText, tokens);

            return tokens;
        }
    }
}
