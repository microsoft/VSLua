using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormattingEngine.Ruling
{
    internal class Rule
    {
        private readonly RuleDescriptor ruleDescriptor;
        private readonly RuleOperation ruleOperation;

        internal Rule(RuleDescriptor ruleDescriptor, ContextFilter[] contextFilters, RuleAction action)
        {
            this.ruleDescriptor = ruleDescriptor;
            this.ruleOperation = new RuleOperation(new RuleOperationContext(contextFilters), action);
        }

        internal bool Applies(FormattingContext formattingContext)
        {


            return ruleOperation.Context.InContext(formattingContext);
        }

    }
}
