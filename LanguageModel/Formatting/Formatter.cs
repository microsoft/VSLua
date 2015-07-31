using System.Collections.Generic;
using LanguageService.Formatting.Options;
using LanguageService.Formatting.Ruling;
using Validation;

namespace LanguageService.Formatting
{
    public sealed class Formatter
    {
        internal Formatter(ParseTreeCache parseTreeProvider)
        {
            Requires.NotNull(parseTreeProvider, nameof(parseTreeProvider));

            this.parseTreeProvider = parseTreeProvider;
            globalOptions = new GlobalOptions();
            ruleMap = RuleMap.Create();
        }

        private ParseTreeCache parseTreeProvider;
        private GlobalOptions globalOptions;
        private RuleMap ruleMap;

        /// <summary>
        /// This is main entry point for the VS side of things. For now, the implementation
        /// of the function is not final and it just used as a way seeing results in VS.
        /// Ideally, Format will also take in a "formatting option" object that dictates
        /// the rules that should be enabled, spacing and tabs.
        /// </summary>
        /// <returns>
        /// A list of TextEditInfo objects are returned for the spacing between tokens (starting from the
        /// first token in the document to the last token. After the spacing text edits, the indentation
        /// text edits follow (starting again from the beginning of the document). I might separate the
        /// indentation text edits from the spacing text edits in the future but for now they are in
        /// the same list.
        /// </returns>
        public List<TextEditInfo> Format(SourceText span, NewOptions newOptions)
        {
            if (newOptions != null)
            {
                globalOptions = new GlobalOptions(newOptions);
                ruleMap = RuleMap.Create(globalOptions.OptionalRuleMap);
            }

            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            List<Token> tokens = parseTreeProvider.Get(span);

            List<ParsedToken> parsedTokens = ParsedToken.GetParsedTokens(tokens);

            for (int i = 0; i < parsedTokens.Count - 1; ++i)
            {
                FormattingContext formattingContext =
                    new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                Rule rule = ruleMap.Get(formattingContext);

                if (rule != null)
                {
                    textEdits.AddRange(rule.Apply(formattingContext));
                }
            }

            textEdits.AddRange(Indenter.GetIndentations(parsedTokens));

            return textEdits;
        }
    }
}
