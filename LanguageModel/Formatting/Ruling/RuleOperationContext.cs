using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{

    internal delegate bool ContextFilter(ref FormattingContext formattingContext);

    internal class RuleOperationContext
    {
        private List<ContextFilter> contextFilters;

        internal RuleOperationContext(List<ContextFilter> contextFilters)
        {
            this.contextFilters = contextFilters;
        }

        internal bool InContext(ref FormattingContext formattingContext)
        {
            foreach (ContextFilter contextFilter in contextFilters)
            {
                if (!contextFilter(ref formattingContext))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
