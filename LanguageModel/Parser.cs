using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Immutable;
using Xunit;

namespace LanguageModel
{
    public enum Context
    {
        IfBlockContext,
        ProgramContext,
        ElseBlock,
        ElseIfBlock,
    }
    public class Parser
    {
        private Stack<Context> contextStack;
        private Token currentToken;
        private List<Token> tokenList;
        private int positionInTokenList;
        private List<ParseError> errorList;

        Parser()
        {
            contextStack = new Stack<Context>();
            errorList = new List<ParseError>();
            positionInTokenList = -1; //TODO? bad practice?
        }

        #region tokenList Accessors
        private Token NextToken()
        {
            if(positionInTokenList < tokenList.Count)
            {
                currentToken = tokenList[++positionInTokenList];
            }
            return currentToken;
        }

        private bool ParseExpected(TokenType type)
        {
            int temp = positionInTokenList;
            if(tokenList[++temp].Type == type)
            {
                currentToken = NextToken();
                return true;
            } else
            {
                //TODO: parse parent contexts?
                return false;
            }
        }
        #endregion

        private bool IsContextTerminator(Context parsingContext, TokenType currentTokenType)
        {
            switch (parsingContext)
            {
                case Context.IfBlockContext:
                    return (currentTokenType == TokenType.EndKeyword || currentTokenType == TokenType.ElseIfKeyword || currentTokenType == TokenType.ElseKeyword);
                case Context.ElseBlock:
                    return (currentTokenType == TokenType.EndKeyword);
                case Context.ElseIfBlock:
                    return (currentTokenType == TokenType.ElseIfKeyword || currentTokenType == TokenType.EndKeyword);
                case Context.ProgramContext:
                    return currentTokenType == TokenType.EndOfFile;
                default:
                    throw new Exception("Unknown Context");
            }
        }

        public SyntaxTree CreateSyntaxTree(string filename)
        {
            Stream luaStream = File.OpenRead(filename);
            tokenList = Lexer.Tokenize(luaStream);
            ChunkNode root = ParseChunkNode();
            return new SyntaxTree(filename, root, errorList);
        }

        private ChunkNode ParseChunkNode()
        {
            int start = currentToken.FullStart;
            Block block = ParseBlock(Context.ProgramContext);
            int length = currentToken.Start + currentToken.Length - start;

            Assert.NotEqual(TokenType.EndOfFile, currentToken.Type);

            return ChunkNode.Create(start, length, block, currentToken);
        }

        //TODO: deal with edge case where there is nothing contained within the block
        private Block ParseBlock(Context parsingContext)
        {
            contextStack.Push(parsingContext);
            int start = this.NextToken().FullStart;
            List<SyntaxNode> children = new List<SyntaxNode>();
            while (this.IsContextTerminator(parsingContext, currentToken.Type))
            {
                children.Add(ParseStatement());
            }

            int length = currentToken.Start + currentToken.Length - start;

            contextStack.Pop();
            return Block.Create(start, length, children.ToImmutableList());
        }


        private SyntaxNode ParseStatement()
        {
            switch (currentToken.Type)
            {
                case TokenType.BreakKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.GotoKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.DoKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.WhileKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.RepeatKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.IfKeyword:
                    return ParseIfNode();
                case TokenType.ForKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.FunctionKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.LocalKeyword:
                    throw new NotImplementedException();
                    break;
                case TokenType.Identifier:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
                    //not the beginning of a statement?
                    break;
            }
            return null;//TODO: remove this
        }

        private IfNode ParseIfNode()
        {
            int start = currentToken.Start;
            Token ifKeyword = currentToken;
            Expression expression = this.ParseExpression();

            //TODO: do context analysis vs. just returning a missing token?
            Token thenKeyword = ParseExpected(TokenType.ThenKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);
            
            int length = currentToken.Start + currentToken.Length - start;

            Block ifBlock = this.ParseBlock(Context.IfBlockContext);
            List<ElseIfBlock> elseIfList = ParseElseIfList();
            ElseBlock elseBlock = ParseElseBlock();

            if (elseIfList == null && elseBlock == null)
            {
                return IfNode.Create(start, length, ifKeyword, expression, thenKeyword, ifBlock);
            }
            else
            {
                return IfNode.Create(start, length, ifKeyword, expression, thenKeyword, ifBlock, elseIfList.ToImmutableList(), elseBlock);
            }
        }

        private Expression ParseExpression()
        {
            throw new NotImplementedException();
        }

        private ElseBlock ParseElseBlock()
        {
            if(this.ParseExpected(TokenType.ElseKeyword))
            {
                int start = currentToken.Start;
                Token elseKeyword = currentToken;
                Block elseBlock = ParseBlock(Context.ElseBlock);
                int length = currentToken.Start + currentToken.Length - start;
                return ElseBlock.Create(start, length, elseKeyword, elseBlock);
            } else
            {
                return null;
            }
        }

        private List<ElseIfBlock> ParseElseIfList()
        {
            List<ElseIfBlock> elseIfList = new List<ElseIfBlock>();

            while (ParseExpected(TokenType.ElseIfKeyword))
            {
                elseIfList.Add(this.ParseElseIfBlock());
            }

            return elseIfList;
        }

        private ElseIfBlock ParseElseIfBlock()
        {
            int start = currentToken.Start;
            Token elseIfKeyword = currentToken;
            Expression exp = ParseExpression();
            Token thenKeyword = ParseExpected(TokenType.ThenKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);
            Block block = ParseBlock(Context.ElseIfBlock);
            int length = currentToken.Start + currentToken.Length - start;
            return ElseIfBlock.Create(start, length, elseIfKeyword, exp, thenKeyword, block);
        }
    }
}