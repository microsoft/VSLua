using System.Collections.Generic;
using LanguageService.Formatting.Options;
using Validation;

namespace LanguageService.Formatting
{
    internal class Indenter
    {
        internal static IEnumerable<TextEditInfo> GetIndentations(List<ParsedToken> parsedTokens, GlobalOptions globalOptions)
        {
            Requires.NotNull(parsedTokens, nameof(parsedTokens));

            foreach (ParsedToken parsedToken in parsedTokens)
            {
                string indentationString =
                                  Indenter.GetIndentationStringFromBlockLevel(parsedToken, globalOptions);
                foreach (IndentInfo indentInfo in Indenter.GetIndentInformation(parsedToken))
                {
                    yield return new TextEditInfo(indentInfo.Start, indentInfo.Length, indentationString);
                }
            }
        }

        private struct IndentInfo
        {
            internal IndentInfo(int start, int length)
            {
                this.Start = start;
                this.Length = length;
            }

            internal int Start { get; }
            internal int Length { get; }
        }

        private static string GetIndentationStringFromBlockLevel(ParsedToken parsedToken, GlobalOptions globalOptions)
        {
            int totalSpaces = parsedToken.BlockLevel * (int)globalOptions.IndentSize;

            int spacesNeeded = totalSpaces;
            int tabsNeeded = 0;

            if (globalOptions.UsingTabs && globalOptions.TabSize > 0)
            {
                spacesNeeded = totalSpaces % (int)globalOptions.TabSize;
                tabsNeeded = (totalSpaces - spacesNeeded) / (int)globalOptions.TabSize;
            }

            return Indenter.MakeIndentation(tabsNeeded, spacesNeeded);
        }

        private static string MakeIndentation(int tabsNeeded, int spacesNeeded)
        {
            return new string('\t', tabsNeeded) + new string(' ', spacesNeeded);
        }

        private static IEnumerable<IndentInfo> GetIndentInformation(ParsedToken parsedToken)
        {
            Requires.NotNull(parsedToken, nameof(parsedToken));

            List<Trivia> leadingTrivia = parsedToken.Token.LeadingTrivia;

            if (leadingTrivia.Count <= 0)
            {
                yield break;
            }

            if (parsedToken.Token.FullStart == 0 && leadingTrivia[0].Type == SyntaxKind.Whitespace)
            {
                yield return new IndentInfo(parsedToken.Token.FullStart, leadingTrivia[0].Text.Length);
            }

            int start = parsedToken.Token.FullStart;

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                start += leadingTrivia[i].Text.Length;

                Trivia currentTrivia = leadingTrivia[i];
                if (currentTrivia.Type != SyntaxKind.Newline)
                {
                    continue;
                }

                if (i + 1 >= leadingTrivia.Count ||
                           leadingTrivia[i + 1].Type != SyntaxKind.Whitespace)
                {
                    yield return new IndentInfo(start, 0);
                }
                else
                {
                    Trivia nextTrivia = leadingTrivia[i + 1];
                    if (nextTrivia.Type == SyntaxKind.Whitespace)
                    {
                        yield return new IndentInfo(start, nextTrivia.Text.Length);
                    }
                }
            }
        }
    }
}
