/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using Validation;

namespace LanguageService.Formatting.Options
{
    public class FormattingOptions
    {

        /// <summary>
        /// Updates the general options.
        /// </summary>
        /// <param name="disableRuleGroups">
        /// The rules that can be turned off for the formatter.
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
        public FormattingOptions(
            List<DisableableRules> disableRuleGroups,
            uint tabSize, uint indentSize, bool usingTabs)
        {
            Requires.NotNull(disableRuleGroups, nameof(disableRuleGroups));

            this.TabSize = tabSize;
            this.IndentSize = indentSize;
            this.UsingTabs = usingTabs;
            this.RuleGroupsToDisable = disableRuleGroups.ToImmutableArray();
            this.OptionalRuleMap = new OptionalRuleMap(this.RuleGroupsToDisable);
        }

        public FormattingOptions()
        {
            this.IndentSize = 4;
            this.OptionalRuleMap = new OptionalRuleMap(ImmutableArray.Create<DisableableRules>());
        }

        public uint IndentSize { get; }

        public uint TabSize { get; }

        public bool UsingTabs { get; }

        public ImmutableArray<DisableableRules> RuleGroupsToDisable { get; }

        internal OptionalRuleMap OptionalRuleMap { get; }
    }
}
