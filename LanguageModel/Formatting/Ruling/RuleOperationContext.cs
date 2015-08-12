using System;
using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleOperationContext
    {
        private List<Func<FormattingContext, bool>> contextFilters;

        internal RuleOperationContext(List<Func<FormattingContext, bool>> contextFilters)
        {
            Validation.Requires.NotNull(contextFilters, nameof(contextFilters));

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
