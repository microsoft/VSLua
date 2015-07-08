using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormattingEngine.Ruling
{

    internal delegate bool ContextFilter(FormattingContext formattingContext);

    internal class RuleOperationContext
    {
        private ContextFilter[] contextFilters;

        internal RuleOperationContext(ContextFilter[] contextFilters)
        {
            this.contextFilters = contextFilters;
        }

        internal bool InContext(FormattingContext formattingContext)
        {
            foreach (ContextFilter contextFilter in contextFilters)
            {
                if (!contextFilter(formattingContext))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
