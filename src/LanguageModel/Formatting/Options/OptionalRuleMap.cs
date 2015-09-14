/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageService.Formatting.Ruling;

namespace LanguageService.Formatting.Options
{
    /// <summary>
    /// OptionalRuleMap holds all the Rules that can be turned off, and is sent in as a parameter
    /// when the Rules are changed in "Update" in GlobalOptions.
    /// </summary>
    internal class OptionalRuleMap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "<Pending>")]
        internal readonly HashSet<Rule> DisabledRules = new HashSet<Rule>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "<Pending>")]
        internal readonly HashSet<DisableableRules> DisabledRuleGroups = new HashSet<DisableableRules>();

        /// <summary>
        /// Allows Rule disabling.
        /// </summary>
        /// <param name="optionalRuleGroups">
        /// The OptionalRuleGroups that are to be disabled/skipped.
        /// </param>
        internal OptionalRuleMap(IEnumerable<DisableableRules> optionalRuleGroups)
        {
            Validation.Requires.NotNull(optionalRuleGroups, nameof(optionalRuleGroups));
            foreach (DisableableRules group in optionalRuleGroups)
            {
                this.Disable(group);
            }
        }

        private void Disable(DisableableRules optionalRuleGroup)
        {
            IEnumerable<Rule> ruleGroup;

            if (!optionalRuleGroups.TryGetValue(optionalRuleGroup, out ruleGroup))
            {
                return;
            }

            this.AddRuleGroup(ruleGroup);
        }

        private void AddRuleGroup(IEnumerable<Rule> ruleGroup)
        {
            Validation.Requires.NotNull(ruleGroup, nameof(ruleGroup));
            this.DisabledRules.UnionWith(ruleGroup);
        }

#pragma warning disable SA1306 // Field names must begin with lower-case letter
        private static ImmutableArray<Rule> SpaceBeforeOpenParenthesis = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeOpenParenthesis);

        private static ImmutableArray<Rule> SpaceOnInsideOfParenthesis = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseParenthesis,
            Rules.SpaceBeforeValueAfterOpenParenthesis);

        private static ImmutableArray<Rule> SpaceOnInsideOfCurlyBraces = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseCurlyBrace,
            Rules.SpaceBeforeValueAfterOpenCurlyBrace);

        private static ImmutableArray<Rule> SpaceOnInsideOfSquareBrackets = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseSquareBracket,
            Rules.SpaceBeforeValueAfterOpenSquareBracket);

        private static ImmutableArray<Rule> SpaceAfterCommas = ImmutableArray.Create(
            Rules.SpaceAfterComma);

        private static ImmutableArray<Rule> SpaceBeforeAndAfterBinaryOperations = ImmutableArray.Create(
            Rules.SpaceAfterBinaryOperator,
            Rules.SpaceBeforeBinaryOperator);

        // TODO: This Rule Group expands after the parser implemented, check OptionalRuleGroup enum
        //   for the rest
        private static ImmutableArray<Rule> SpaceBeforeAndAfterAssignmentInStatement = ImmutableArray.Create(
            Rules.SpaceAfterAssignmentOperatorInStatement,
            Rules.SpaceBeforeAssignmentOperatorInStatement);

        private static ImmutableArray<Rule> SpaceBeforeAndAfterAssignmentInFor = ImmutableArray.Create(
            Rules.SpaceBeforeAssignmentOperatorInFor,
            Rules.SpaceAfterAssignmentOperatorInFor);

        private static ImmutableArray<Rule> NoSpaceAfterCommaInFor = ImmutableArray.Create(
            Rules.NoSpaceAfterCommaInFor);

        private static ImmutableArray<Rule> SpaceBeforeAfterAssignmentInField = ImmutableArray.Create(
            Rules.SpaceBeforeAssignmentOperatorInField,
            Rules.SpaceAfterAssignmentOperatorInField);
#pragma warning restore SA1306 // Field names must begin with lower-case letter

        private static Dictionary<DisableableRules, IEnumerable<Rule>> optionalRuleGroups = new Dictionary<DisableableRules, IEnumerable<Rule>>
        {
            { DisableableRules.SpaceBeforeOpenParenthesis, SpaceBeforeOpenParenthesis },
            { DisableableRules.SpaceOnInsideOfParenthesis, SpaceOnInsideOfParenthesis },
            { DisableableRules.SpaceOnInsideOfCurlyBraces, SpaceOnInsideOfCurlyBraces },
            { DisableableRules.SpaceOnInsideOfSquareBrackets, SpaceOnInsideOfSquareBrackets },
            { DisableableRules.SpaceAfterCommas, SpaceAfterCommas },
            { DisableableRules.SpaceBeforeAndAfterBinaryOperations, SpaceBeforeAndAfterBinaryOperations },
            { DisableableRules.SpaceBeforeAndAfterAssignmentForStatement, SpaceBeforeAndAfterAssignmentInStatement },
            { DisableableRules.SpaceBeforeAndAfterAssignmentInForLoopHeader, SpaceBeforeAndAfterAssignmentInFor },
            { DisableableRules.NoSpaceBeforeAndAfterIndiciesInForLoopHeader, NoSpaceAfterCommaInFor },
            { DisableableRules.SpaceBeforeAndAfterAssignmentForField, SpaceBeforeAfterAssignmentInField }
        };
    }
}
