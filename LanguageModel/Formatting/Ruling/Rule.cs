using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal abstract class Rule
    {
        internal abstract RuleDescriptor RuleDescriptor { get; }
        internal abstract RuleOperation RuleOperationContext { get; }

        internal abstract bool AppliesTo(FormattingContext formattingContext);
        internal abstract List<TextEditInfo> Apply(FormattingContext formattingContext);
    }
}
