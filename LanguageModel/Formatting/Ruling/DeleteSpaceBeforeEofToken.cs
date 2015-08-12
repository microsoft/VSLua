using System;
using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class DeleteSpaceBeforeEofToken : SimpleRule
    {
        internal DeleteSpaceBeforeEofToken() : base(
                new RuleDescriptor(TokenRange.All, SyntaxKind.EndOfFile),
                new List<Func<FormattingContext, bool>> { Rules.TokensAreOnSameLine },
                RuleAction.Delete)
        {
        }

        // formattingContext is a struct, and therefore not nullable.
        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            List<TextEditInfo> edits = new List<TextEditInfo>();
            TextEditInfo edit = this.GetLastSpaceTriviaInfo(formattingContext.NextToken.Token);

            if (edit != null)
            {
                edits.Add(edit);
            }

            return edits;
        }

        private TextEditInfo GetLastSpaceTriviaInfo(Token token)
        {
            Validation.Requires.NotNull(token, nameof(token));
            List<Trivia> leadingTrivia = token.LeadingTrivia;
            if (leadingTrivia == null || leadingTrivia.Count == 0)
            {
                return null;
            }

            Trivia lastTrivia = leadingTrivia[token.LeadingTrivia.Count - 1];

            TextEditInfo edit = null;

            if (lastTrivia.Type == SyntaxKind.Whitespace)
            {
                int length = lastTrivia.Text.Length;
                int start = token.Start - length;
                edit = new TextEditInfo(start, length, "");
            }

            return edit;
        }

    }
}
