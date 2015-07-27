using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService.Formatting.Ruling;

namespace LanguageService.Formatting.Options
{
    /// <summary>
    /// OptionalRuleMap holds all the disabled Rules, and it sent in as a parameter when the Rules
    /// are changed in "UpdateRuleMap" in Formatter.
    /// </summary>
    public class OptionalRuleMap
    {
        internal HashSet<Rule> DisabledRules = new HashSet<Rule>();
        internal HashSet<OptionalRuleGroup> DisabledRuleGroups = new HashSet<OptionalRuleGroup>();

        /// <summary>
        /// Allows Rule disabling.
        /// </summary>
        /// <param name="optionalRuleGroups">
        /// The OptionalRuleGroups that are to be disabled/skipped.
        /// </param>
        public OptionalRuleMap(IEnumerable<OptionalRuleGroup> optionalRuleGroups)
        {
            DisabledRuleGroups.Clear();
            DisabledRules.Clear();

            foreach (OptionalRuleGroup group in optionalRuleGroups)
            {
                Disable(group);
            }

        }

        private void Disable(OptionalRuleGroup optionalRuleGroup)
        {
            IEnumerable<Rule> ruleGroup;

            switch (optionalRuleGroup)
            {
                case OptionalRuleGroup.SpaceBeforeOpenParenthesis:
                    ruleGroup = SpaceBeforeOpenParenthesis;
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfParenthesis:
                    ruleGroup = SpaceOnInsideOfParenthesis;
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfCurlyBraces:
                    ruleGroup = SpaceOnInsideOfCurlyBraces;
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfSquareBrackets:
                    ruleGroup = SpaceOnInsideOfSquareBrackets;
                    break;

                case OptionalRuleGroup.SpaceAfterCommas:
                    ruleGroup = SpaceAfterCommas;
                    break;

                case OptionalRuleGroup.SpaceBeforeAndAfterBinaryOperations:
                    ruleGroup = SpaceBeforeAndAfterBinaryOperations;
                    break;

                case OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForStatement:
                    ruleGroup = SpaceBeforeAndAfterAssignment;
                    break;

                default:
                    throw new NotImplementedException();
            }

            AddRuleGroup(ruleGroup);
        }

        private void AddRuleGroup(IEnumerable<Rule> ruleGroup)
        {
            Validation.Requires.NotNull(ruleGroup, nameof(ruleGroup));

            foreach (Rule rule in ruleGroup)
            {
                DisabledRules.Add(rule);
            }
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
        private static ImmutableArray<Rule> SpaceBeforeAndAfterAssignment = ImmutableArray.Create(
            Rules.SpaceAfterAssignmentOperator,
            Rules.SpaceBeforeAssignmentOperator
            );
    }
}
