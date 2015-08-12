using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageService.Formatting.Ruling;

namespace LanguageService.Formatting.Options
{
    /// <summary>
    /// OptionalRuleMap holds all the disabled Rules, and it sent in as a parameter when the Rules
    /// are changed in "Update" in GlobalOptions.
    /// </summary>
    internal class OptionalRuleMap
    {
        internal readonly HashSet<Rule> DisabledRules = new HashSet<Rule>();
        internal readonly HashSet<OptionalRuleGroup> DisabledRuleGroups = new HashSet<OptionalRuleGroup>();

        /// <summary>
        /// Allows Rule disabling.
        /// </summary>
        /// <param name="optionalRuleGroups">
        /// The OptionalRuleGroups that are to be disabled/skipped.
        /// </param>
        internal OptionalRuleMap(IEnumerable<OptionalRuleGroup> optionalRuleGroups)
        {
            Validation.Requires.NotNull(optionalRuleGroups, nameof(optionalRuleGroups));
            foreach (OptionalRuleGroup group in optionalRuleGroups)
            {
                this.Disable(group);
            }
        }

        private void Disable(OptionalRuleGroup optionalRuleGroup)
        {
            IEnumerable<Rule> ruleGroup;

            if (!optionalRuleGroups.TryGetValue(optionalRuleGroup, out ruleGroup))
            {
                throw new NotImplementedException();
            }

            this.AddRuleGroup(ruleGroup);
        }

        private void AddRuleGroup(IEnumerable<Rule> ruleGroup)
        {
            Validation.Requires.NotNull(ruleGroup, nameof(ruleGroup));
            this.DisabledRules.UnionWith(ruleGroup);
        }

        private static ImmutableArray<Rule> SpaceBeforeOpenParenthesis = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeOpenParenthesis
            );

        private static ImmutableArray<Rule> SpaceOnInsideOfParenthesis = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseParenthesis,
            Rules.SpaceBeforeValueAfterOpenParenthesis
            );

        private static ImmutableArray<Rule> SpaceOnInsideOfCurlyBraces = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseCurlyBrace,
            Rules.SpaceBeforeValueAfterOpenCurlyBrace
            );

        private static ImmutableArray<Rule> SpaceOnInsideOfSquareBrackets = ImmutableArray.Create(
            Rules.SpaceAfterValueBeforeCloseSquareBracket,
            Rules.SpaceBeforeValueAfterOpenSquareBracket
            );

        private static ImmutableArray<Rule> SpaceAfterCommas = ImmutableArray.Create(
            Rules.SpaceAfterComma
            );

        private static ImmutableArray<Rule> SpaceBeforeAndAfterBinaryOperations = ImmutableArray.Create(
            Rules.SpaceAfterBinaryOperator,
            Rules.SpaceBeforeBinaryOperator
            );

        // TODO: This Rule Group expands after the parser implemented, check OptionalRuleGroup enum
        //   for the rest
        private static ImmutableArray<Rule> SpaceBeforeAndAfterAssignmentInStatement = ImmutableArray.Create(
            Rules.SpaceAfterAssignmentOperatorInStatement,
            Rules.SpaceBeforeAssignmentOperatorInStatement
            );

        private static ImmutableArray<Rule> SpaceBeforeAndAfterAssignmentInFor = ImmutableArray.Create(
            Rules.SpaceBeforeAssignmentOperatorInFor,
            Rules.SpaceAfterAssignmentOperatorInFor
            );

        private static ImmutableArray<Rule> SpaceBeforeAfterAssignmentInField = ImmutableArray.Create(
            Rules.SpaceBeforeAssignmentOperatorInField,
            Rules.SpaceAfterAssignmentOperatorInField
            );

        private static Dictionary<OptionalRuleGroup, IEnumerable<Rule>> optionalRuleGroups = new Dictionary<OptionalRuleGroup, IEnumerable<Rule>>
        {
            {OptionalRuleGroup.SpaceBeforeOpenParenthesis, SpaceBeforeOpenParenthesis},
            {OptionalRuleGroup.SpaceOnInsideOfParenthesis, SpaceOnInsideOfParenthesis},
            {OptionalRuleGroup.SpaceOnInsideOfCurlyBraces, SpaceOnInsideOfCurlyBraces},
            {OptionalRuleGroup.SpaceOnInsideOfSquareBrackets, SpaceOnInsideOfSquareBrackets},
            {OptionalRuleGroup.SpaceAfterCommas, SpaceAfterCommas},
            {OptionalRuleGroup.SpaceBeforeAndAfterBinaryOperations, SpaceBeforeAndAfterBinaryOperations},
            {OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForStatement, SpaceBeforeAndAfterAssignmentInStatement},
            {OptionalRuleGroup.NoSpaceBeforeAndAfterIndiciesInForLoopHeader, SpaceBeforeAndAfterAssignmentInFor},
            {OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForField, SpaceBeforeAfterAssignmentInField}
        };
    }
}
