using LanguageService.Formatting.Ruling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageService.Formatting
{
    public static class Formatter
    {
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static List<TextEditInfo> Format(string span)
        {

            RuleMap ruleMap = Rules.GetRuleMap();
            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            List<Token> tokens = Lexer.Tokenize(GenerateStreamFromString(span));
            List<ParsedToken> parsedTokens = ParsedToken.GetParsedTokens(tokens);

            for (int i = 0; i < parsedTokens.Count - 1; ++i)
            {
                FormattingContext formattingContext =
                    new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                Rule rule = ruleMap.GetRule(formattingContext);

                if (rule != null)
                {
                    /* test */
                    textEdits.Add(rule.Apply(formattingContext));
                }
            }

            Indenter.GetIndentations(parsedTokens, textEdits);

            return textEdits;
        }
    }
}
