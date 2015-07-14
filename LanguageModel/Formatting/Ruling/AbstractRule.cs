using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal abstract class AbstractRule
    {
        internal abstract RuleDescriptor RuleDescriptor { get; }
        internal abstract RuleOperation RuleOperationContext { get; }

        internal abstract bool AppliesTo(FormattingContext formattingContext);
        internal abstract TextEditInfo Apply(FormattingContext formattingContext);
    }
}
