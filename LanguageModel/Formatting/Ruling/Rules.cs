using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class Rules
    {

        // A few test rules for testing

        internal static Rule SpaceAfterComma =
            new Rule(
                new RuleDescriptor(TokenType.Comma, TokenRange.Any),
                new List<ContextFilter> { TokensAreOnSameLine },
                RuleAction.Space);

        internal static Rule SpaceAfterAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenRange.Any, TokenType.AssignmentOperator),
                new List<ContextFilter> { TokensAreOnSameLine },
                RuleAction.Space);

        internal static Rule SpaceBeforeAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenRange.Any, TokenType.AssignmentOperator),
                new List<ContextFilter> { TokensAreOnSameLine },
                RuleAction.Space);



        internal static bool TokensAreOnSameLine(FormattingContext formattingContext)
        {
            return formattingContext.TokensOnSameLine();
        }

    }
}
