using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService.Formatting.Ruling;

namespace LanguageService.Formatting.Options
{
    public static class OptionalRuleMap
    {
        private delegate void AddRemoveFunc(IEnumerable<Rule> ruleGroup);

        internal static HashSet<Rule> DisabledRules = new HashSet<Rule>();
        internal static HashSet<OptionalRuleGroup> DisabledRuleGroups = new HashSet<OptionalRuleGroup>();

        /// <summary>
        /// Allows Rule disabling.
        /// </summary>
        /// <param name="optionalRuleGroup">
        /// The OptionalRuleGroup that is to be disabled/skipped.
        /// </param>
        public static void Disable(OptionalRuleGroup optionalRuleGroup)
        {
            EnableDisableGeneral(optionalRuleGroup, true);
        }

        /// <summary>
        /// Allows Rule enabling
        /// </summary>
        /// <param name="optionalRuleGroup">
        /// The OptionRuleGroup that is to be enabled/applied.
        /// </param>
        public static void Enable(OptionalRuleGroup optionalRuleGroup)
        {
            EnableDisableGeneral(optionalRuleGroup, false);
        }

        private static void EnableDisableGeneral(OptionalRuleGroup optionalRuleGroup, bool disabling)
        {
            AddRemoveFunc addRemoveFunc;
            bool alreadyDisabled = DisabledRuleGroups.Contains(optionalRuleGroup);

            if (disabling)
            {
                if (alreadyDisabled)
                {
                    return;
                }

                addRemoveFunc = AddRuleGroup;
            }
            else
            {
                if (!alreadyDisabled)
                {
                    return;
                }

                addRemoveFunc = DeleteRuleGroup;
            }

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

            addRemoveFunc(ruleGroup);
        }

        private static void AddRuleGroup(IEnumerable<Rule> ruleGroup)
        {
            Validation.Requires.NotNull(ruleGroup, nameof(ruleGroup));

            foreach (Rule rule in ruleGroup)
            {
                if (!DisabledRules.Contains(rule))
                {
                    DisabledRules.Add(rule);
                }
            }
        }

        private static void DeleteRuleGroup(IEnumerable<Rule> ruleGroup)
        {
            foreach (Rule rule in ruleGroup)
            {
                if (DisabledRules.Contains(rule))
                {
                    DisabledRules.Remove(rule);
                }
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
