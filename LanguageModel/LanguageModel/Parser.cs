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
        ExpListContext,
        FuncBodyContext,
    }
    public class Parser
    {
        private Stack<Context> contextStack;
        private Token currentToken;
        private List<Token> tokenList;
        private int positionInTokenList;
        private List<ParseError> errorList;

        public Parser()
        {
            contextStack = new Stack<Context>();
            errorList = new List<ParseError>();
            positionInTokenList = -1; //TODO? bad practice?
        }

        public SyntaxTree CreateSyntaxTree(string filename)
        {
            Stream luaStream = File.OpenRead(filename);
            tokenList = Lexer.Tokenize(luaStream);
            ChunkNode root = ParseChunkNode();
            return new SyntaxTree(filename, root, errorList);
        }

        #region tokenList Accessors
        private Token NextToken()
        {
            if (positionInTokenList < tokenList.Count)
            {
                currentToken = tokenList[++positionInTokenList];
            }
            return currentToken;
        }

        private bool ParseExpected(TokenType type)
        {
            int temp = positionInTokenList;
            if (tokenList[++temp].Type == type)
            {
                currentToken = NextToken();
                return true;
            }
            else
            {
                //TODO: parse parent contexts?
                return false;
            }
        }

        private Token Peek()
        {
            if (positionInTokenList + 1 < tokenList.Count)
            {
                return tokenList[positionInTokenList + 1];
            }
            else
            {
                return null;
            }
        }

        private bool PushBack()
        {
            if(positionInTokenList - 1 >= 0)
            {
                positionInTokenList--;
                currentToken = tokenList[positionInTokenList];
                return true;
            } else
            {
                return false;
            }
        }
        #endregion

        private bool IsContextTerminator(Context parsingContext, TokenType currentTokenType)
        {
            if(currentTokenType == TokenType.ReturnKeyword)
            {
                return true;
            }

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
                    throw new Exception("Unknown Context"); //TODO
            }
        }

        private ChunkNode ParseChunkNode()
        {
            int start = Peek().FullStart;
            Block block = ParseBlock(Context.ProgramContext);
            int length = currentToken.Start + currentToken.Length - start;

            Assert.NotEqual(TokenType.EndOfFile, currentToken.Type);

            return ChunkNode.Create(start, length, block, currentToken);
        }

        private Block ParseBlock(Context parsingContext)
        {
            //TODO: consider an exit if invalid block beginner token?
            //TODO: deal with edge case where there is nothing contained within the block

            contextStack.Push(parsingContext);
            int start = this.Peek().FullStart;
            List<SyntaxNode> children = new List<SyntaxNode>();

            while (!IsContextTerminator(parsingContext, Peek().Type))
            {
                children.Add(ParseStatement());
            }

            if(Peek().Type == TokenType.ReturnKeyword)
            {
                RetStat retstat = ParseRetStat();
                int length = currentToken.Start + currentToken.Length - start;
                contextStack.Pop();

                return Block.Create(start, length, children.ToImmutableList(),retstat);
            } else
            {
                int length = currentToken.Start + currentToken.Length - start;
                contextStack.Pop();

                return Block.Create(start, length, children.ToImmutableList());
            }
        }

        private RetStat ParseRetStat()
        {
            if(ParseExpected(TokenType.ReturnKeyword))
            {
                return null;
            }

            int start = currentToken.Start;
            Token returnKeyword = currentToken;
            ExpList expList = ParseExpList();
            int length = currentToken.Start + currentToken.Length - start;
            if(expList == null)
            {
                return RetStat.Create(start, length, returnKeyword);
            } else
            {
                return RetStat.Create(start, length, returnKeyword, expList);
            }
        }

        private ExpList ParseExpList()
        {
            int start = Peek().Start;
            contextStack.Push(Context.ExpListContext);
            List<ExpressionCommaPair> expressions = new List<ExpressionCommaPair>();
            bool searchForExps;

            do
            {
                Expression exp = ParseExpression();

                if(exp == null)
                {
                    if(expressions.Count == 0)
                    {
                        return MissingNode.Create(start,0).ToExpList(null); //TODO: is this okay???????
                    } else
                    {
                        expressions.Add(ExpressionCommaPair.Create(MissingNode.Create(currentToken.FullStart, 0).ToExpression(), null)); //TODO: is this okay???????
                        searchForExps = false;
                    }
                }

                if (Peek().Type == TokenType.Comma)
                {
                    expressions.Add(ExpressionCommaPair.Create(exp, NextToken()));
                    searchForExps = true;
                } else
                {
                    expressions.Add(ExpressionCommaPair.Create(exp, null));
                    searchForExps = false;
                }
            } while (searchForExps);

            int length = currentToken.FullStart - start;

            contextStack.Pop();
            return ExpList.Create(start, length, expressions.ToImmutableList());
        }



        private SyntaxNode ParseStatement()
        {//TODO check if it follows contract to recive position where current is already consumed?
            switch (Peek().Type)
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
                case TokenType.Identifier: //TODO remove
                    var tempNode = NextToken();
                    return VarArgPar.Create(tempNode.Start, tempNode.Length, tempNode);
                default: //TODO remove
                    var tempNode2 = NextToken();
                    return VarArgPar.Create(tempNode2.Start, tempNode2.Length, tempNode2);
            }
        }

        //pass in null if this is the first time calling the method... if not, this can be called recursively.
        private Expression ParseExpression()
        {
            Expression exp;
            int start = Peek().Start;
            if (ParseExpected(TokenType.LengthUnop)
                || ParseExpected(TokenType.NotUnop)
                || ParseExpected(TokenType.MinusOperator) //TODO: ambiguity of tilde bug?
                || ParseExpected(TokenType.TildeUnOp)) //TODO: ambiguity of tilde bug?
            {
                var node = UnopExpression.CreateBuilder();
                node.StartPosition = currentToken.Start;
                node.Unop = currentToken;
                node.Exp = ParseExpression().ToBuilder();
                exp = node.ToImmutable();
            } else
            {
                switch (Peek().Type)
                {
                    case TokenType.NilKeyValue:
                    case TokenType.FalseKeyValue:
                    case TokenType.TrueKeyValue:
                    case TokenType.Number:
                    case TokenType.String:
                    case TokenType.VarArgOperator:
                        exp = SimpleExpression.Create(NextToken().Start, currentToken.Length, currentToken);
                        break;
                    case TokenType.FunctionKeyword:
                        exp = ParseFunctionDef().ToExpression(); //TODO: is this okay????
                        break;
                    case TokenType.OpenParen:
                        exp = null;
                        //TODOexp = ParseParenPrefixExp;
                        break;
                    case TokenType.OpenCurlyBrace:
                        exp = null;
                        //TODO: exp = ParseTableConstructor();
                        break;
                    case TokenType.Identifier:
                        //TODO:var or prefix exp
                        exp = null;
                        break;
                    default:
                        //TODO:
                        exp = null;
                        break;
                }
            }

            if (IsBinop(Peek().Type))
            {
                var binop = NextToken();
                return BinopExpression.Create(start, binop.Start + binop.Length - start, exp, binop, ParseExpression());
            } else
            {
                return exp;
            }
          
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = ParseExpected(TokenType.FunctionKeyword) ? currentToken : Token.CreateMissingToken(Peek().FullStart - 1);
            node.FunctionBody = ParseFunctionBody().ToBuilder();
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();            
        }

        private FuncBody ParseFunctionBody()
        {
            var node = FuncBody.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenParen = ParseExpected(TokenType.OpenParen) ? currentToken : Token.CreateMissingToken(Peek().FullStart - 1);
            node.ParameterList = ParseParList().ToBuilder() ;
            node.CloseParen = ParseExpected(TokenType.CloseParen) ? currentToken : Token.CreateMissingToken(Peek().FullStart - 1);
            node.Block = ParseBlock(Context.FuncBodyContext).ToBuilder();
            node.EndKeyword = ParseExpected(TokenType.EndKeyword) ? currentToken : Token.CreateMissingToken(Peek().FullStart - 1);
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private ParList ParseParList()
        {
            //TODO: implement;
            return null;
        }

        private bool IsBinop(TokenType type)
        {
            switch(type)
            {
                case TokenType.PlusOperator:
                case TokenType.MinusOperator:
                case TokenType.MultiplyOperator:
                case TokenType.DivideOperator:
                case TokenType.FloorDivideOperator:
                case TokenType.ExponentOperator:
                case TokenType.ModulusOperator:
                case TokenType.TildeUnOp: //TODO: deal with ambiguity
                case TokenType.BitwiseAndOperator:
                case TokenType.BitwiseOrOperator:
                case TokenType.BitwiseRightOperator:
                case TokenType.BitwiseLeftOperator:
                case TokenType.NotEqualsOperator:
                case TokenType.LessOrEqualOperator:
                case TokenType.GreaterOrEqualOperator:
                case TokenType.EqualityOperator:
                case TokenType.StringConcatOperator:
                case TokenType.GreaterThanOperator: 
                case TokenType.LessThanOperator:
                case TokenType.AndBinop:
                case TokenType.OrBinop:
                    return true;
                default:
                    return false;
            }
        }

        private IfNode ParseIfNode()
        {
            var node = IfNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.IfKeyword = ParseExpected(TokenType.IfKeyword) ? currentToken : Token.CreateMissingToken(node.StartPosition);
            node.Exp = ParseExpression().ToBuilder();

            //TODO: do context analysis vs. just returning a missing token?
            node.ThenKeyword = ParseExpected(TokenType.ThenKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);

            node.IfBlock = ParseBlock(Context.IfBlockContext).ToBuilder();

            //Block ifBlock = this.ParseBlock(Context.IfBlockContext);
            //List<ElseIfBlock> elseIfList = ParseElseIfList();
            //ElseBlock elseBlock = ParseElseBlock();

            node.EndKeyword = ParseExpected(TokenType.EndKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);

            int length = currentToken.Start + currentToken.Length - node.StartPosition;

            return node.ToImmutable();
        }

        //private ElseBlock ParseElseBlock()
        //{
        //    if (this.ParseExpected(TokenType.ElseKeyword))
        //    {
        //        int start = currentToken.Start;
        //        Token elseKeyword = currentToken;
        //        Block elseBlock = ParseBlock(Context.ElseBlock);
        //        int length = currentToken.Start + currentToken.Length - start;
        //        return ElseBlock.Create(start, length, elseKeyword, elseBlock);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //private List<ElseIfBlock> ParseElseIfList()
        //{
        //    List<ElseIfBlock> elseIfList = new List<ElseIfBlock>();

        //    while (ParseExpected(TokenType.ElseIfKeyword))
        //    {
        //        elseIfList.Add(this.ParseElseIfBlock());
        //    }

        //    return elseIfList;
        //}

        //private ElseIfBlock ParseElseIfBlock()
        //{
        //    int start = currentToken.Start;
        //    Token elseIfKeyword = currentToken;
        //    Expression exp = ParseExpression(null);
        //    Token thenKeyword = ParseExpected(TokenType.ThenKeyword) ? currentToken : Token.CreateMissingToken(currentToken.Start + currentToken.Length);
        //    Block block = ParseBlock(Context.ElseIfBlock);
        //    int length = currentToken.Start + currentToken.Length - start;
        //    return ElseIfBlock.Create(start, length, elseIfKeyword, exp, thenKeyword, block);
        //}
    }
}