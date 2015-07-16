using LanguageService.Formatting.Ruling;
using System.Collections.Generic;
using System.IO;

namespace LanguageService.Formatting
{
    public static class Formatter
    {
        /*
        public static List<TextEditInfo> Format(string span)
        {
            return Format(Tester.)
        }*/

        /// <summary>
        /// This is main entry point for the VS side of things. For now, the implementation
        /// of the function is not final and it just used as a way seeing results in VS.
        /// Ideally, Format will also take in a "formatting option" object that dictates
        /// the rules that should be enabled, spacing and tabs.
        /// </summary>
        /// <returns>
        /// A list of TextEditInfo objects are returned for the spacing between tokens (starting from the
        /// first token in the document to the last token. After the spacing text edits, the indentation
        /// text edits follow (starting again from the beginning of the document). I might separate the
        /// indentation text edits from the spacing text edits in the future but for now they are in
        /// the same list.
        /// </returns>
        public static List<TextEditInfo> Format(Stream span)
        {
            RuleMap ruleMap = Rules.GetRuleMap();
            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            List<Token> tokens = Lexer.Tokenize(span);
            List<ParsedToken> parsedTokens = ParsedToken.GetParsedTokens(tokens);

            for (int i = 0; i < parsedTokens.Count - 1; ++i)
            {
                FormattingContext formattingContext =
                    new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                Rule rule = ruleMap.Get(formattingContext);

                if (rule != null)
                {
                    foreach (TextEditInfo textEdit in rule.Apply(formattingContext))
                    {
                        textEdits.Add(textEdit);
                    }
                }
            }

            foreach (TextEditInfo edit in Indenter.GetIndentations(parsedTokens))
            {
                textEdits.Add(edit);
            }
            return textEdits;
        }
    }
}
