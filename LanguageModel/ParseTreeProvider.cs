using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService;
using System.Runtime.CompilerServices;
using System.IO;

namespace LanguageModel
{
    public static class ParseTreeProvider
    {
        private static ConditionalWeakTable<SourceText, List<Token>> sources = 
            new ConditionalWeakTable<SourceText, List<Token>>();

        public static List<Token> Get(SourceText sourceText)
        {
            List<Token> tokens = null;
            if (sources.TryGetValue(sourceText, out tokens))
            {
                return tokens;
            }
            tokens = Lexer.Tokenize(sourceText.textReader);
            sources.Add(sourceText, tokens);
            return tokens;
        }
    }
}
