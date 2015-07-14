using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal interface IRule
    {
        RuleDescriptor RuleDescriptor { get; }
        RuleOperation RuleOperationContext { get; }

        bool AppliesTo(FormattingContext formattingContext);
        TextEditInfo Apply(FormattingContext formattingContext);
    }
}
