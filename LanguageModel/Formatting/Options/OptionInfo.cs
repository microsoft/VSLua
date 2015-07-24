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
        /// <summary>
        /// The genernal rule options for the formatter
        /// </summary>
        /// <param name="enabledRuleGroups">
        /// The enabled rules for the formatter.
        /// </param>
        /// <param name="disabledRuleGroups">
        /// The disabled rules for the formatter.
        /// </param>
        /// <param name="indentSize">
        /// How big in spaces the indents are.
        /// </param>
        /// <param name="indentStyleInfo">The indentation information for the Indenter.</param>
        public OptionInfo(
            List<OptionalRuleGroup> enabledRuleGroups,
            List<OptionalRuleGroup> disabledRuleGroups,
            uint indentSize,
            IndentStyleInfo indentStyleInfo)
        {
            this.IndentSize = indentSize;
            this.EnabledRuleGroups = enabledRuleGroups.ToImmutableArray();
            this.DisabledRuleGroups = disabledRuleGroups.ToImmutableArray();
            this.IndentStyleInfo = indentStyleInfo;
        }

        internal uint IndentSize { get; }
        internal ImmutableArray<OptionalRuleGroup> EnabledRuleGroups { get; }
        internal ImmutableArray<OptionalRuleGroup> DisabledRuleGroups { get; }
        internal IndentStyleInfo IndentStyleInfo { get; }
    }
}
