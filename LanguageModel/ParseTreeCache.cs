using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Validation;


namespace LanguageService
{
    internal class ParseTreeCache
    {
        private readonly ConditionalWeakTable<SourceText, List<Token>> sources = 
            new ConditionalWeakTable<SourceText, List<Token>>();

        internal List<Token> Get(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            List<Token> tokens;
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
