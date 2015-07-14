using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal class Indenter
    {


        internal static void GetIndentations(List<ParsedToken> parsedTokens, List<TextEditInfo> edits)
        {
            foreach (ParsedToken parsedToken in parsedTokens)
            {
                int[] lastNewline = Indenter.GetLastNewlinePositionAndLength(parsedToken);

                if (lastNewline != null)
                {
                    string indentationString =
                        Indenter.GetIndentationFromBlockLevel(parsedToken.BlockLevel, null);

                    edits.Add(new TextEditInfo(lastNewline[0], lastNewline[1], indentationString));
                }
            }
        }

        private static string GetIndentationFromBlockLevel(int blockLevel, SyntaxNode syntaxNode)
        {
            return Indenter.MakeIndentation(blockLevel * 4); // TODO: hardcoded for now
        }

        // TODO: Make it actually do something, pretty hardcoded.
        private static string MakeIndentation(int spaces)
        {
            string indentation = "";
            for (int i = 0; i < spaces; ++i)
            {
                indentation += ' ';
            }
            return indentation;
        }

        private static int[] GetLastNewlinePositionAndLength(ParsedToken parsedToken)
        {
            int length = 0;
            int start = parsedToken.Token.FullStart;
            bool foundNewline = false;

            var leadingTrivia = parsedToken.Token.LeadingTrivia;

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                if (leadingTrivia[i].Type == Trivia.TriviaType.Newline)
                {
                    foundNewline = true;
                    start += leadingTrivia[i].Text.Length;
                    length = 0;
                    if (i + 1 < leadingTrivia.Count)
                    {
                        length = leadingTrivia[i + 1].Text.Length;
                    }
                }
            }

            if (foundNewline)
            {
                return new int[2] { start, length };
            }
            return null;
        }
    }
}
