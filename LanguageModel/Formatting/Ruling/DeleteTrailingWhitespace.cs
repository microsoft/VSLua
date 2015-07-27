using System;
using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class DeleteTrailingWhitespace : SimpleRule
    {
        internal DeleteTrailingWhitespace() :
            base(
                new RuleDescriptor(TokenRange.All, TokenRange.All),
                new List<Func<FormattingContext, bool>> { Rules.TokensAreNotOnSameLine },
                RuleAction.Delete)
        {
        }

        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            Token nextToken = formattingContext.NextToken.Token;
            // flag for the end of file token where whitespace should be deleted
            List<TextEditInfo> edits = this.GetEdits(nextToken, nextToken.Type == TokenType.EndOfFile);

            return edits;
        }

        private List<TextEditInfo> GetEdits(Token token, bool isEndOfFile)
        {

            List<TextEditInfo> edits = new List<TextEditInfo>();

            int start = token.FullStart;
            int length = 0;
            var leadingTrivia = token.LeadingTrivia;
            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                length = leadingTrivia[i].Text.Length;
                if (this.IsNewLineAfterSpace(i, leadingTrivia) ||
                    (isEndOfFile && this.IsSpaceBeforeEndOfFile(i, leadingTrivia)))
                {
                    edits.Add(new TextEditInfo(start, length, string.Empty));
                }

                start += length;
            }

            return edits;
        }

        private bool IsNewLineAfterSpace(int index, List<Trivia> triviaList)
        {
            if (index + 1 >= triviaList.Count)
            {
                return false;
            }

            return triviaList[index].Type == Trivia.TriviaType.Whitespace &&
                   triviaList[index + 1].Type == Trivia.TriviaType.Newline;
        }

        private bool IsSpaceBeforeEndOfFile(int index, List<Trivia> triviaList)
        {
            if (index >= triviaList.Count)
            {
                return false;
            }

            return triviaList[index].Type == Trivia.TriviaType.Whitespace;
        }
    }
}
