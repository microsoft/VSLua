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
        internal ContextFilter[] contextFilters;

        internal RuleOperationContext(ContextFilter[] contextFilters)
        {
            this.contextFilters = contextFilters;
        }

    }
}
