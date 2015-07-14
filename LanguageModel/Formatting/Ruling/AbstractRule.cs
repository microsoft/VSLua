using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal abstract class AbstractRule
    {
        public abstract RuleDescriptor RuleDescriptor { get; }
        public abstract RuleOperation RuleOperationContext { get; }

        public abstract bool AppliesTo(FormattingContext formattingContext);
        public abstract TextEditInfo Apply(FormattingContext formattingContext);
    }
}
