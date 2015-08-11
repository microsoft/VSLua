using System.Collections.Generic;
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
            this.globalOptions = new GlobalOptions();
            this.ruleMap = RuleMap.Create();
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
        public List<TextEditInfo> Format(SourceText sourceText, Range range, NewOptions newOptions)
        {
            if (newOptions != null)
            {
                this.globalOptions = new GlobalOptions(newOptions);
                this.ruleMap = RuleMap.Create(globalOptions.OptionalRuleMap);
            }

            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            SyntaxTree syntaxTree = this.parseTreeProvider.Get(sourceText);

            List<ParsedToken> parsedTokens = new List<ParsedToken>(ParsedToken.GetParsedTokens(syntaxTree, range));

            for (int i = 0; i < parsedTokens.Count - 1; ++i)
            {
                FormattingContext formattingContext =
                    new FormattingContext(parsedTokens[i], parsedTokens[i + 1]);

                Rule rule = this.ruleMap.Get(formattingContext);

                if (rule != null)
                {
                    foreach (TextEditInfo edit in rule.Apply(formattingContext))
                    {
                        textEdits.Add(edit);
                    }
                }
            }

            textEdits.AddRange(Indenter.GetIndentations(parsedTokens, globalOptions));

            return textEdits;
        }

        public int SmartIndent(SourceText sourceText, int position)
        {
            SyntaxTree syntaxTree = this.parseTreeProvider.Get(sourceText);
            return Indenter.GetIndentationFromPosition(syntaxTree, this.globalOptions, position);
        }

        public bool PositionInSyntaxKind(SourceText sourceText, int position, SyntaxKind syntaxKind)
        {
            SyntaxNodeOrToken currentNode = this.parseTreeProvider.Get(sourceText).Root;

            while (true)
            {
                if (currentNode as Token != null)
                {
                    return ((Token)currentNode).Kind == syntaxKind;
                }

                SyntaxNode syntaxNode = (SyntaxNode)currentNode;

                if (syntaxNode.Kind == syntaxKind)
                {
                    return true;
                }

                if (syntaxNode.Children == null || syntaxNode.Children.Count == 0)
                {
                    return syntaxNode.Kind == syntaxKind;
                }

                if (syntaxNode.Children.Count < 2)
                {
                    currentNode = syntaxNode.Children[0];
                    continue;
                }

                for (int i = 0; i < syntaxNode.Children.Count - 1; ++i)
                {
                    SyntaxNodeOrToken currentChild = syntaxNode.Children[i];
                    SyntaxNodeOrToken nextChild = syntaxNode.Children[i + 1];

                    int startCurrentChild = currentChild as SyntaxNode == null ?
                        ((Token)currentChild).FullStart :
                        ((SyntaxNode)currentChild).StartPosition;

                    int startNextChild = nextChild as SyntaxNode == null ?
                        ((Token)nextChild).Start :
                        ((SyntaxNode)nextChild).StartPosition;

                    if (position > startCurrentChild && position < startNextChild)
                    {
                        currentNode = currentChild;
                        break;
                    }

                    if (i + 1 >= syntaxNode.Children.Count - 1)
                    {
                        currentNode = nextChild;
                        break;
                    }

                }
            }
        }
    }
}
