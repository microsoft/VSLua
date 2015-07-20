using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal class ParsedToken
    {
        internal Token Token { get; }
        internal int BlockLevel { get; }
        internal SyntaxNode Node { get; }


        private static readonly ImmutableArray<TokenType> IncreaseIndentAfter = ImmutableArray.Create
            (
                TokenType.DoKeyword,
                TokenType.ThenKeyword,
                TokenType.ElseKeyword,
                TokenType.FunctionKeyword,
                TokenType.OpenCurlyBrace
            );

        private static readonly ImmutableArray<TokenType> DecreaseIndentOn = ImmutableArray.Create
            (
                TokenType.EndKeyword,
                TokenType.ElseIfKeyword,
                TokenType.CloseCurlyBrace,
                TokenType.ElseKeyword
            );

        internal ParsedToken(Token token, int blockLevel, SyntaxNode node)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token null");
            }
            this.Token = token;
            this.BlockLevel = blockLevel;
            this.Node = node;
        }

        // This function wouldn't exist in the final version since instead of iterating
        //   through all the tokens from the lexer, I'd just walk the parsetree from the start
        internal static List<ParsedToken> GetParsedTokens(List<Token> tokens, int from, int to)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException();
            }
            List<ParsedToken> parsedTokens = new List<ParsedToken>();

            int indent_level = 0;
            foreach (Token token in tokens)
            {


                if (token.FullStart > to)
                {
                    break;
                }


                if (DecreaseIndentOn.Contains(token.Type))
                {
                    indent_level--;
                }

                indent_level = indent_level < 0 ? 0 : indent_level;
                if (token.Start >= from)
                {
                    parsedTokens.Add(new ParsedToken(token, indent_level, null));
                }
                if (IncreaseIndentAfter.Contains(token.Type))
                {
                    indent_level++;
                }

                if (token.Type == TokenType.ReturnKeyword)
                {
                    indent_level--;
                }
            }
            return parsedTokens;
        }
    }
}
