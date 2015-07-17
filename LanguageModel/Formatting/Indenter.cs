using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal class Indenter
    {
        private struct IndentInfo
        {
            internal int Start { get; }
            internal int Length { get; }
            internal IndentInfo(int start, int length)
            {
                this.Start = start;
                this.Length = length;
            }
        }

        internal static IEnumerable<TextEditInfo> GetIndentations(List<ParsedToken> parsedTokens)
        {
            if (parsedTokens == null)
            {
                throw new ArgumentNullException();
            }
            foreach (ParsedToken parsedToken in parsedTokens)
            {
                IndentInfo? indentInfo = Indenter.GetSpacePositionAndLengthAfterLastNewline(parsedToken);

                if (indentInfo != null)
                {
                    string indentationString =
                        Indenter.GetIndentationStringFromBlockLevel(parsedToken.BlockLevel, null);
                    yield return new TextEditInfo(((IndentInfo)indentInfo).Start, ((IndentInfo)indentInfo).Length, indentationString);
                }
            }
        }

        private static string GetIndentationStringFromBlockLevel(int blockLevel, SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
            {
                //throw new ArgumentNullException();
            }

            // Here I would put the calculation for the indentation string parts
            // how many tabs, spaces that I need. I would also check the options for
            // how tabs are setup.
            return Indenter.MakeIndentation(blockLevel * 4);
        }


        // This is for the actual construction of the indentation, the actual string.
        // For now the parameter just takes how many spaces are needed.
        private static string MakeIndentation(int spaces)
        {
            return new string(' ', spaces);
        }

        private static IndentInfo? GetSpacePositionAndLengthAfterLastNewline(ParsedToken parsedToken)
        {
            if (parsedToken == null)
            {
                throw new ArgumentNullException();
            }
            int length = 0;
            int start = parsedToken.Token.FullStart;

            int currentStart = parsedToken.Token.FullStart;
            bool foundNewline = false;

            var leadingTrivia = parsedToken.Token.LeadingTrivia;

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                if (leadingTrivia[i].Type == Trivia.TriviaType.Newline)
                {
                    foundNewline = true;
                    start = currentStart + leadingTrivia[i].Text.Length;
                    length = 0;
                    if (i + 1 < leadingTrivia.Count &&
                        leadingTrivia[i + 1].Type == Trivia.TriviaType.Whitespace)
                    {
                        length = leadingTrivia[i + 1].Text.Length;
                    }
                }

                currentStart += leadingTrivia[i].Text.Length;
            }

            if (foundNewline)
            {
                return new IndentInfo(start, length);
            }
            return null;
        }
    }
}
