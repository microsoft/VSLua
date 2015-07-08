using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class Rule
    {
        internal readonly RuleDescriptor ruleDescriptor;
        internal readonly RuleOperation ruleOperation;

        internal Rule(RuleDescriptor ruleDescriptor, ContextFilter[] contextFilters, RuleAction action)
        {
            this.ruleDescriptor = ruleDescriptor;
            this.ruleOperation = new RuleOperation(new RuleOperationContext(contextFilters), action);

        }

        internal bool AppliesTo(FormattingContext formattingContext)
        {


            return ruleOperation.Context.InContext(formattingContext);
        }

    }
}
