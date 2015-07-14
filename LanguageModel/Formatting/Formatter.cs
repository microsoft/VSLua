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

            RuleMap ruleMap = new RuleMap();
            ruleMap.AddRule(Rules.SpaceAfterComma);
            ruleMap.AddRule(Rules.SpaceAfterAssignmentOperator);
            ruleMap.AddRule(Rules.SpaceAfterBinaryOperator);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseCurlyBrace);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseParenthesis);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseSquareBracket);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeOpenParenthesis);
            ruleMap.AddRule(Rules.SpaceBeforeAssignmentOperator);
            ruleMap.AddRule(Rules.SpaceBeforeBinaryOperator);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenCurlyBrace);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenParenthesis);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenSquareBracket);
            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            IEnumerable<Token> tokens = Lexer.Tokenize(GenerateStreamFromString(span));
            List<ParsedToken> parsedTokens = new List<ParsedToken>();

            foreach (Token token in tokens)
            {
                parsedTokens.Add(new ParsedToken(token, 0, null));
            }

            for (int i = 0; i < parsedTokens.Count - 1; ++i)
            {
                FormattingContext formattingContext =
                    new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                Rule rule = ruleMap.GetRule(formattingContext);

                if (rule != null)
                {
                    textEdits.Add(rule.Apply(formattingContext));

          
                }

            }

            return textEdits;
        }
    }
}
