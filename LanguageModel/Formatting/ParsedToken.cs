using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal sealed class ParsedToken
    {

        internal Token Token { get; private set; }
        internal int BlockLevel { get; private set; }
        internal SyntaxNode Node { get; private set; }

        internal ParsedToken(Token token, int blockLevel, SyntaxNode node)
        {
            this.Token = token;
            this.BlockLevel = blockLevel;
            this.Node = node;
        }

        internal static List<ParsedToken> GetParsedTokens(List<Token> tokens)
        {
            List<ParsedToken> parsedTokens = new List<ParsedToken>();

            List<SyntaxKind> IncreaseIndentAfter = new List<SyntaxKind>
            {
                SyntaxKind.DoKeyword,
                SyntaxKind.ThenKeyword,
                SyntaxKind.ElseKeyword,
                SyntaxKind.FunctionKeyword,
                SyntaxKind.OpenCurlyBrace
            };

            List<SyntaxKind> DecreaseIndentOn = new List<SyntaxKind>
            {
                SyntaxKind.EndKeyword,
                SyntaxKind.ElseIfKeyword,
                SyntaxKind.CloseCurlyBrace,
                SyntaxKind.ElseKeyword
            };

            int indent_level = 0;

            foreach (Token token in tokens)
            {
                if (DecreaseIndentOn.Contains(token.Kind))
                {
                    indent_level--;
                }

                indent_level = indent_level < 0 ? 0 : indent_level;

                parsedTokens.Add(new ParsedToken(token, indent_level, null));

                if (IncreaseIndentAfter.Contains(token.Kind))
                {
                    indent_level++;
                }
            }


            return parsedTokens;
        }

    }
}
