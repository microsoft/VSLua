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
        //ExplistContext,
        //ParlistContext,
        //FieldlistContext,
        //ExpressionContext,
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

        private bool IsContextTerminator(Context parsingContext, TokenType currentTokentType)
        {
            switch(parsingContext)
            {
                case Context.IfBlockContext:
                    return (currentTokentType == TokenType.EndKeyword || currentTokentType == TokenType.ElseIfKeyword || currentTokentType == TokenType.ElseKeyword);
                case Context.ProgramContext:
                    return currentTokentType == TokenType.EndOfFile;
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
            Token thenKeyword = this.ParseExpected(TokenType.ThenKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);
            
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
            throw new NotImplementedException();
        }

        private List<ElseIfBlock> ParseElseIfList()
        {
            throw new NotImplementedException();
        }

        

        //private ElseIfBlock ParseElseIfBlocks(List<Token> tokenEnumerator)
        //{
        //    ElseIfBlock.Builder elseifBlockBuilder = ElseIfBlock.CreateBuilder();
        //    //elseifBlockBuilder.ExtractTokenInfo(tokenEnumerator.Current);

        //    elseifBlockBuilder.ElseIfKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
        //    if (!tokenEnumerator.MoveNext())
        //    {
        //        //TODO: Error?
        //    }

        //    elseifBlockBuilder.Exp = ParseExp(tokenEnumerator, TokenType.ThenKeyword);

        //    elseifBlockBuilder.ThenKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
        //    if (!tokenEnumerator.MoveNext())
        //    {
        //        //TODO: Error?
        //    }

        //    elseifBlockBuilder.Block = ParseIfBlock(tokenEnumerator);

        //    //TODO: elseifBlockBuilder.Length = tokenEnumerator.Current.FullStart - elseifBlockBuilder.FullStartPosition;

        //    return elseifBlockBuilder.ToImmutable();
        //}

        //private Expression.Builder ParseExp(List<Token> tokenEnumerator, TokenType terminatingKeyword)
        //{
        //    Expression.Builder expBuilder = Expression.CreateBuilder();
        //    expBuilder.ExtractTokenInfo(tokenEnumerator.Current);

        //    while(tokenEnumerator.Current.Type != terminatingKeyword)
        //    {
        //        expBuilder.Keyvalue = KeyValue.CreateBuilder();
        //        expBuilder.Keyvalue.ExtractTokenInfo(tokenEnumerator.Current);
        //        expBuilder.Keyvalue.Value = tokenEnumerator.Current.Text;
        //        if (!tokenEnumerator.MoveNext())
        //        {
        //            //TODO: Error?
        //        }
        //    }
        //    expBuilder.Length = tokenEnumerator.Current.FullStart - expBuilder.FullStartPosition;
        //    return expBuilder;
        //}



        //private EndOfFileNode.Builder ParseEndOfFile(List<Token> tokenEnumerator)
        //{
        //    EndOfFileNode.Builder chunkNodeBuilder = EndOfFileNode.CreateBuilder();
        //    chunkNodeBuilder.ExtractTokenInfo(tokenEnumerator.Current);
        //    return chunkNodeBuilder;
        //}



        //private Block.Builder ParseIfBlock(List<Token> tokenEnumerator)
        //{
        //    Block.Builder ifBlockBuilder = Block.CreateBuilder();
        //    ifBlockBuilder.ExtractTokenInfo(tokenEnumerator.Current);
        //    while ((tokenEnumerator.Current.Type != TokenType.ElseKeyword 
        //    || tokenEnumerator.Current.Type != TokenType.ElseIfKeyword
        //    || tokenEnumerator.Current.Type != TokenType.EndKeyword) 
        //    && tokenEnumerator.Current.Type != TokenType.EndOfFile) //TODO: Formating?
        //    {
        //        ifBlockBuilder.Children.Add(ParseStatement(tokenEnumerator));
        //        if (!tokenEnumerator.MoveNext())
        //        {
        //            //TODO: Error?
        //        }
        //    }
        //    ifBlockBuilder.Length = tokenEnumerator.Current.FullStart - ifBlockBuilder.FullStartPosition;

        //    return ifBlockBuilder;
        //}
    }
}