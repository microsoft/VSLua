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
        /// <param name="tabSize">
        /// How big in spaces the indents are when you format
        /// </param>
        /// <param name="indentSize">
        /// How big in spaces the indents are when you press tab,
        /// </param>
        /// <param name="usingTabs">
        /// Whether or not Keep Tabs is on or off.
        /// </param>
        public NewOptions(
            List<OptionalRuleGroup> disableRuleGroups,
            uint tabSize, uint indentSize, bool usingTabs)
        {
            Requires.NotNull(disableRuleGroups, nameof(disableRuleGroups));

            this.RuleGroupsToDisable = disableRuleGroups.ToImmutableArray();
            this.TabSize = tabSize;
            this.IndentSize = indentSize;
            this.UsingTabs = usingTabs;
        }

        internal ImmutableArray<OptionalRuleGroup> RuleGroupsToDisable { get; }
        internal uint TabSize { get; }
        internal uint IndentSize { get; }
        internal bool UsingTabs { get; }
    }
}
