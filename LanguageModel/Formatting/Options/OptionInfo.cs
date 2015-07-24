using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Options
{
    public class OptionInfo
    {
        public OptionInfo(uint tabSize, List<OptionalRuleGroup> optionRuleGroups)
        {
            this.TabSize = tabSize;
            this.OptionalRuleGroups = optionRuleGroups.ToImmutableArray();
        }

        internal uint TabSize { get; }
        internal ImmutableArray<OptionalRuleGroup> OptionalRuleGroups { get; }
    }
}
