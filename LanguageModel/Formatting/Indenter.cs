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
                string indentationString =
                        Indenter.GetIndentationStringFromBlockLevel(parsedToken.BlockLevel, null);
                foreach (IndentInfo indentInfo in Indenter.GetIndentInfos(parsedToken))
                {
                    yield return new TextEditInfo(indentInfo.Start, indentInfo.Length, indentationString);
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
        private static string MakeIndentation(int amount)
        {
            return new string(' ', amount);
        }

        private static IEnumerable<IndentInfo> GetIndentInfos(ParsedToken parsedToken)
        {
            if (parsedToken == null)
            {
                throw new ArgumentNullException();
            }

            List<Trivia> leadingTrivia = parsedToken.Token.LeadingTrivia;

            if (leadingTrivia.Count > 0)
            {
                if (parsedToken.Token.FullStart == 0 && leadingTrivia[0].Type == Trivia.TriviaType.Whitespace)
                {
                    yield return new IndentInfo(parsedToken.Token.FullStart, leadingTrivia[0].Text.Length);
                }

                int start = parsedToken.Token.FullStart;

                for (int i = 0; i < leadingTrivia.Count; ++i)
                {
                    Trivia currentTrivia = leadingTrivia[i];
                    if (currentTrivia.Type == Trivia.TriviaType.Newline)
                    {
                        int indentStart = start + currentTrivia.Text.Length;
                        if (i + 1 >= leadingTrivia.Count ||
                            leadingTrivia[i + 1].Type != Trivia.TriviaType.Whitespace)
                        {
                            yield return new IndentInfo(indentStart, 0);
                        }
                        else
                        {
                            Trivia nextTrivia = leadingTrivia[i + 1];
                            if (nextTrivia.Type == Trivia.TriviaType.Whitespace)
                            {
                                yield return new IndentInfo(indentStart, nextTrivia.Text.Length);
                            }
                        }
                    }
                    start += leadingTrivia[i].Text.Length;
                }
            }

        }
    }
}
