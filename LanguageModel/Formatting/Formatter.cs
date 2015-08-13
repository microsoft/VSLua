/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LanguageService.Formatting.Options;
using LanguageService.Formatting.Ruling;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Formatting
{
    public sealed class Formatter
    {
        internal Formatter(ParseTreeCache parseTreeProvider)
        {
            Requires.NotNull(parseTreeProvider, nameof(parseTreeProvider));

            this.parseTreeProvider = parseTreeProvider;
            this.formattingOptions = new FormattingOptions();
            this.ruleMap = RuleMap.Create();
        }

        private ParseTreeCache parseTreeProvider;
        private FormattingOptions formattingOptions;
        private RuleMap ruleMap;

        /// <summary>
        /// This is main entry point for the VS side of things. For now, the implementation
        /// of the function is not final and it just used as a way seeing results in VS.
        /// Ideally, Format will also take in a "formatting option" object that dictates
        /// the rules that should be enabled, spacing and tabs.
        /// </summary>
        /// <param name="sourceText">The SourceText that represents the text to be formatted</param>
        /// <param name="range">The range of indicies to be formatted</param>
        /// <param name="formattingOptions">The options to format with, null leaves the options as they were</param>
        /// <returns>
        /// A list of TextEditInfo objects are returned for the spacing between tokens (starting from the
        /// first token in the document to the last token. After the spacing text edits, the indentation
        /// text edits follow (starting again from the beginning of the document). I might separate the
        /// indentation text edits from the spacing text edits in the future but for now they are in
        /// the same list.
        /// </returns>
        public List<TextEditInfo> Format(SourceText sourceText, Range range, FormattingOptions formattingOptions)
        {
            Requires.NotNull(formattingOptions, nameof(formattingOptions));
            Requires.NotNull(sourceText, nameof(sourceText));

            this.formattingOptions = formattingOptions;
            this.ruleMap = RuleMap.Create(this.formattingOptions.OptionalRuleMap);

            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            SyntaxTree syntaxTree = this.parseTreeProvider.Get(sourceText);

            List<ParsedToken> parsedTokens = new List<ParsedToken>(ParsedToken.GetParsedTokens(syntaxTree, range));

            if (syntaxTree.ErrorList.Count == 0)
            {
                for (int i = 0; i < parsedTokens.Count - 1; ++i)
                {
                    FormattingContext formattingContext =
                        new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                    Rule rule = this.ruleMap.Get(formattingContext);

                    if (rule != null)
                    {
                        textEdits.AddRange(rule.Apply(formattingContext));
                    }
                }
            }

            textEdits.AddRange(Indenter.GetIndentations(parsedTokens, this.formattingOptions));

            textEdits.Sort((x, y) => x.Start < y.Start ? 1 : x.Start == y.Start ? 0 : -1);

            return textEdits;
        }

        /// <summary>
        /// Gets the indentation level at the specified position in the source text.
        /// </summary>
        /// <param name="sourceText">The content for the smart indenting</param>
        /// <param name="position">The position to check for the block level</param>
        /// <returns>The indentation amount in spaces</returns>
        public int SmartIndent(SourceText sourceText, int position)
        {
            SyntaxTree syntaxTree = this.parseTreeProvider.Get(sourceText);
            return Indenter.GetIndentationFromPosition(syntaxTree, this.formattingOptions, position);
        }
    }
}
