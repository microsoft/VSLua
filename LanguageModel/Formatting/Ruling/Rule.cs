using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class Rule
    {
        internal readonly RuleDescriptor ruleDescriptor;
        internal readonly RuleOperation ruleOperation;

        internal Rule(RuleDescriptor ruleDescriptor, List<ContextFilter> contextFilters, RuleAction action)
        {
            this.ruleDescriptor = ruleDescriptor;
            this.ruleOperation = new RuleOperation(new RuleOperationContext(contextFilters), action);

        }

        private string GetTextFromAction()
        {
            switch (ruleOperation.Action)
            {
                case RuleAction.Delete:
                    return "";
                case RuleAction.Newline:
                    return "\n";
                default:
                    return " ";
            }
        }

        internal bool AppliesTo(FormattingContext formattingContext)
        {
            return ruleOperation.Context.InContext(formattingContext);
        }

        // Very simple implentation of Apply
        internal TextEditInfo Apply(FormattingContext formattingContext)
        {

            Token leftToken = formattingContext.CurrentToken.Token;
            Token rightToken = formattingContext.NextToken.Token;

            int start = leftToken.Start + leftToken.Text.Length;
            int length = rightToken.Start - start;
            string replaceWith = this.GetTextFromAction();


            return new TextEditInfo(start, length, replaceWith);
        }

    }
}
