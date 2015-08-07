using System.Collections.Generic;
using System.Collections.Immutable;
using Validation;

namespace LanguageService.Formatting.Options
{

    /// <summary>
    /// This class might be bypassed later since GlobalOptions.Update just takes three arguements.
    /// </summary>
    public class NewOptions
    {
        /// <summary>
        /// Updates the general options.
        /// <param name="disabledRuleGroups">
        /// The disabled rules for the formatter.
        /// </param>
        /// <param name="indentSize">
        /// How big in spaces the indents are.
        /// </param>
        /// <param name="indentStyleInfo">The indentation information for the Indenter.</param>
        public NewOptions(
            List<OptionalRuleGroup> disableRuleGroups,
            uint indentSize,
            IndentStyleInfo indentStyleInfo)
        {
            Requires.NotNull(disableRuleGroups, nameof(disableRuleGroups));
            Requires.NotNull(indentStyleInfo, nameof(indentStyleInfo));

            this.RuleGroupsToDisable = disableRuleGroups.ToImmutableArray();
            this.IndentSize = indentSize;
            this.IndentStyleInfo = indentStyleInfo;
        }

        internal ImmutableArray<OptionalRuleGroup> RuleGroupsToDisable { get; }
        internal uint IndentSize { get; }
        internal IndentStyleInfo IndentStyleInfo { get; }
    }
}
