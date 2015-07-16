using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleOperation
    {
        internal RuleOperationContext Context { get; }
        internal RuleAction Action { get; }

        internal RuleOperation(RuleOperationContext context, RuleAction action)
        {
            this.Action = action;
            this.Context = context;
        }
    }
}
