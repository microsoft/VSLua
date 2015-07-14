using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleOperation
    {
        internal RuleOperationContext Context { get; private set; }
        internal RuleAction Action { get; private set; }

        internal RuleOperation(RuleOperationContext context, RuleAction action)
        {
            this.Action = action;
            this.Context = context;
        }

    }
}
