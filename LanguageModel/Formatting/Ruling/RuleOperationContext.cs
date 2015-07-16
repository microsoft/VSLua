using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleOperationContext
    {
        private List<Func<FormattingContext, bool>> contextFilters;

        internal RuleOperationContext(List<Func<FormattingContext, bool>> contextFilters)
        {
            this.contextFilters = contextFilters;
        }

        internal bool InContext(FormattingContext formattingContext)
        {
            foreach (Func<FormattingContext, bool> contextFilter in contextFilters)
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
