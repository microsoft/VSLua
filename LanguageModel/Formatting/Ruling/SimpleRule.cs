using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class SimpleRule : Rule
    {
        protected readonly RuleDescriptor ruleDescriptor;
        protected readonly RuleOperation ruleOperation;

        internal override RuleDescriptor RuleDescriptor
        {
            get
            {
                return ruleDescriptor;
            }
        }

        internal override RuleOperation RuleOperationContext
        {
            get
            {
                return ruleOperation;
            }
        }

        internal SimpleRule(RuleDescriptor ruleDescriptor, List<Func<FormattingContext, bool>> contextFilters, RuleAction action)
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
                case RuleAction.Space:
                    return " ";
                default:
                    return "";
            }
        }

        internal override bool AppliesTo(FormattingContext formattingContext)
        {
            return ruleOperation.Context.InContext(formattingContext);
        }

        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            Token leftToken = formattingContext.CurrentToken.Token;
            Token rightToken = formattingContext.NextToken.Token;

            int start = leftToken.Start + leftToken.Text.Length;
            int length = rightToken.Start - start;
            string replaceWith = this.GetTextFromAction();

            return new List<TextEditInfo> { new TextEditInfo(start, length, replaceWith) };
        }

    }
}
