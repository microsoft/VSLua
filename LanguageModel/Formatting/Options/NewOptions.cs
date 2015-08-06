using System.Collections.Generic;
using System.Collections.Immutable;
using Validation;
using LanguageService;

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
        public NewOptions(
            List<OptionalRuleGroup> disableRuleGroups,
            uint indentSize)
        {
            Requires.NotNull(disableRuleGroups, nameof(disableRuleGroups));

            this.RuleGroupsToDisable = disableRuleGroups.ToImmutableArray();
            this.IndentSize = indentSize;
        }

        internal ImmutableArray<OptionalRuleGroup> RuleGroupsToDisable { get; }
        internal uint IndentSize { get; }
    }
}
