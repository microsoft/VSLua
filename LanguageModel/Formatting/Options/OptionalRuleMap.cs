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

        public static void Disable(OptionalRuleGroup optionalRuleGroup)
        {
            EnableDisableGeneral(optionalRuleGroup, AddRuleGroup);
        }

        public static void Enable(OptionalRuleGroup optionalRuleGroup)
        {
            EnableDisableGeneral(optionalRuleGroup, DeleteRuleGroup);
        }

        private static void EnableDisableGeneral(OptionalRuleGroup optionalRuleGroup, AddRemoveFunc addRemoveFunc)
        {
            switch (optionalRuleGroup)
            {
                case OptionalRuleGroup.SpaceBeforeOpenParenthesis:
                    addRemoveFunc(SpaceBeforeOpenParenthesis);
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfParenthesis:
                    addRemoveFunc(SpaceOnInsideOfParenthesis);
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfCurlyBraces:
                    addRemoveFunc(SpaceOnInsideOfCurlyBraces);
                    break;

                case OptionalRuleGroup.SpaceOnInsideOfSquareBrackets:
                    addRemoveFunc(SpaceOnInsideOfSquareBrackets);
                    break;

                case OptionalRuleGroup.SpaceAfterCommas:
                    addRemoveFunc(SpaceAfterCommas);
                    break;

                case OptionalRuleGroup.SpaceBeforeAndAfterBinaryOperations:
                    addRemoveFunc(SpaceBeforeAndAfterBinaryOperations);
                    break;

                case OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForStatement:
                    addRemoveFunc(SpaceBeforeAndAfterAssignment);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static void AddRuleGroup(IEnumerable<Rule> ruleGroup)
        {
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
