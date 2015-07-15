using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Immutable;
using Xunit;

namespace LanguageService
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
            positionInTokenList = -1; //Issue: bad practice?
        }

        public SyntaxTree CreateSyntaxTree(string filename)
        {
            Stream luaStream = File.OpenRead(filename);
            tokenList = Lexer.Tokenize(luaStream);
            ChunkNode root = ParseChunkNode();
            return new SyntaxTree(filename, root, errorList);
        }

        //Question: should this be wrapped in its own class?
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
            int tempIndex = positionInTokenList;
            if (tokenList[++tempIndex].Type == type)
            {
                currentToken = NextToken();
                return true;
            }
            else
            {
                //TODO: consider parsing parent contexts?
                return false;
            }
        }

        private Token GetExpectedToken(TokenType type)
        {
            int tempIndex = positionInTokenList;
            if (tokenList[++tempIndex].Type == type)
            {
                currentToken = NextToken();
                return currentToken;
            }
            else
            {
                return Token.CreateMissingToken(currentToken.End);
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
            if (positionInTokenList - 1 >= 0)
            {
                positionInTokenList--;
                currentToken = tokenList[positionInTokenList];
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Parse Methods
        private ChunkNode ParseChunkNode()
        {
            var node = ChunkNode.CreateBuilder();
            node.StartPosition = Peek().FullStart; //Question: okay to use peek? It is assumed here that Index is at -1, contract is that token is only consumed when parser is sure what to do with it.
            node.ProgramBlock = ParseBlock(Context.ProgramContext).ToBuilder();

            if (ParseExpected(TokenType.EndOfFile))
            {
                node.EndOfFile = currentToken;
            }
            else
            {
                node.EndOfFile = Token.CreateMissingToken(currentToken.End); //Question: consider error scenarios
            }
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private Block ParseBlock(Context parsingContext)
        {
            //TODO: consider an exit if invalid block beginner token?
            //TODO: deal with edge case where there is nothing contained within the block

            contextStack.Push(parsingContext);
            var node = Block.CreateBuilder();
            node.StartPosition = this.Peek().FullStart;
            List<SyntaxNode> children = new List<SyntaxNode>();

            while (!IsContextTerminator(parsingContext, Peek().Type))
            {
                children.Add(ParseStatement());
            }

            node.Children = children.ToImmutableList();

            if (Peek().Type == TokenType.ReturnKeyword)
            {
                node.ReturnStatement = ParseRetStat().ToBuilder(); //Question if builder returns null... to bulder pattern?
            }

            node.Length = currentToken.End - node.StartPosition;
            contextStack.Pop();
            return node.ToImmutable();
        }

        private SyntaxNode ParseStatement()
        {//TODO check if it follows contract to recive position where current is already consumed?
            switch (Peek().Type)
            {
                case TokenType.SemiColon:
                    NextToken();
                    return SemiColonStatement.Create(currentToken.Start, currentToken.Length, currentToken); //Question: could just use next token as first param
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
                case TokenType.Identifier: //TODO implement, Misplaced Token node is just a placeholder for now.
                    NextToken();
                    return MisplacedToken.Create(currentToken.Start, currentToken.Length, currentToken);
                default:
                    NextToken();
                    return MisplacedToken.Create(currentToken.Start, currentToken.Length, currentToken);
            }
        }

        private IfNode ParseIfNode()
        {
            var node = IfNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.IfKeyword = GetExpectedToken(TokenType.IfKeyword);
            node.Exp = ParseExpression().ToBuilder();
            node.ThenKeyword = GetExpectedToken(TokenType.ThenKeyword); //Question @cyrus: do context analysis vs. just returning a missing token?
            node.IfBlock = ParseBlock(Context.IfBlockContext).ToBuilder();

            if(Peek().Type == TokenType.ElseIfKeyword)
            {
                node.ElseIfList = ParseElseIfList();
            }

            if (Peek().Type == TokenType.ElseIfKeyword)
            {
                node.ElseBlock = ParseElseBlock().ToBuilder();
            }
                
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private Expression ParseExpression()
        {
            Expression exp;
            int start = Peek().Start;
            if (ParseExpected(TokenType.LengthUnop)
                || ParseExpected(TokenType.NotUnop)
                || ParseExpected(TokenType.MinusOperator) //TODO: deal with ambiguity of minus operator?
                || ParseExpected(TokenType.TildeUnOp)) //TODO: deal with ambiguity of tilde operator?
            {
                var node = UnopExpression.CreateBuilder();
                node.StartPosition = currentToken.Start;
                node.Unop = currentToken;
                node.Exp = ParseExpression().ToBuilder();
                exp = node.ToImmutable();
            }
            else
            {
                switch (Peek().Type)
                {
                    case TokenType.NilKeyValue:
                    case TokenType.FalseKeyValue:
                    case TokenType.TrueKeyValue:
                    case TokenType.Number:
                    case TokenType.String:
                    case TokenType.VarArgOperator:
                        NextToken();
                        exp = SimpleExpression.Create(currentToken.Start, currentToken.Length, currentToken);
                        break;
                    case TokenType.FunctionKeyword:
                        exp = ParseFunctionDef();
                        break;
                    case TokenType.OpenParen:
                        exp = ParseParenPrefixExp();
                        break;
                    case TokenType.OpenCurlyBrace:
                        exp = ParseTableConstructorExp();
                        break;
                    case TokenType.Identifier:
                        exp = ParsePrefixExp();
                        break;
                    default:
                        exp = null; ////Question @cyrus: correct?
                        break;
                }
            }

            if (IsBinop(Peek().Type))
            {
                var binop = NextToken();
                return BinopExpression.Create(start, binop.End - start, exp, binop, ParseExpression()); //Question... this representation is slightly flawed if "operator precedence" matters... but does it matter? What scenario from a tooling perspective cares about precedence?
            }
            else
            {
                return exp;
            }
        }

        private ElseBlock ParseElseBlock()
        {
            if (!ParseExpected(TokenType.ElseKeyword))
            {
                return null;
            }

            var node = ElseBlock.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ElseKeyword = currentToken;
            node.Block = ParseBlock(Context.ElseBlock).ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ImmutableList<ElseIfBlock> ParseElseIfList()
        {
            var elseIfList = new List<ElseIfBlock>();

            while (ParseExpected(TokenType.ElseIfKeyword))
            {
                elseIfList.Add(ParseElseIfBlock());
            }

            return elseIfList.ToImmutableList();
        }

        private ElseIfBlock ParseElseIfBlock()
        {
            if(!ParseExpected(TokenType.ElseIfKeyword))
            {
                return null;
            }
            var node = ElseIfBlock.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ElseIfKeyword = currentToken;
            node.Exp = ParseExpression().ToBuilder();
            node.ThenKeyword = GetExpectedToken(TokenType.ThenKeyword);
            node.Block = ParseBlock(Context.ElseIfBlock).ToBuilder();
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private Expression ParsePrefixExp()
        {
            //TODO: Implement
            //switch (Peek().Type)
            //{

            //}
            return null;
        }

        private RetStat ParseRetStat()
        {
            if (ParseExpected(TokenType.ReturnKeyword))
            {
                return null;
            }

            var node = RetStat.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ReturnKeyword = currentToken;
            node.ReturnExpressions = ParseExpList().ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructor ParseTableConstructor()
        {
            var node = TableConstructor.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(TokenType.OpenCurlyBrace);
            node.FieldList = ParseFieldList().ToBuilder();
            node.CloseCurly = GetExpectedToken(TokenType.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private Expression ParseTableConstructorExp()
        {
            var node = TableConstructorExp.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(TokenType.OpenCurlyBrace);
            node.FieldList = ParseFieldList().ToBuilder();
            node.CloseCurly = GetExpectedToken(TokenType.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FieldList ParseFieldList()
        {
            //TODO: complete implementation... not well tested.
            var node = FieldList.CreateBuilder();
            node.StartPosition = Peek().Start; //TODO: inconsistant start and end...
            bool parseFields = true;
            var fields = new List<FieldAndSeperatorPair>();

            while (parseFields)
            {
                var fieldAndSep = FieldAndSeperatorPair.CreateBuilder();
                fieldAndSep.Field = ParseField().ToBuilder();
                if (ParseExpected(TokenType.Comma) || ParseExpected(TokenType.SemiColon))
                {
                    fieldAndSep.FieldSeparator = currentToken;
                    parseFields = true;
                }
                else
                {
                    if (Peek().Type == TokenType.CloseCurlyBrace)
                    {
                        parseFields = false;
                    }
                    else
                    {
                        fieldAndSep.FieldSeparator = Token.CreateMissingToken(currentToken.End);
                        parseFields = true;
                    }
                    fields.Add(fieldAndSep.ToImmutable());
                }
            }

            node.Fields = fields.ToImmutableList();
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private Field ParseField()
        {
            //TODO implement
            return null;
            //switch (Peek().Type)
            //{
            //    case TokenType.OpenBracket:
            //        return ParseBracketField();
            //    case TokenType.Identifier:
            //        var identifierToken = NextToken();
            //        if (Peek().Type == TokenType.AssignmentOperator)
            //        {

            //        }
            //        else
            //        {

            //        }
            //    default:
            //        return ParseExpField();
            //}
        }

        private ExpField ParseExpField()
        {
            var node = ExpField.CreateBuilder();
            node.StartPosition = currentToken.End;
            node.Exp = ParseExpression().ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private Expression ParseParenPrefixExp()
        {
            var node = ParenPrefixExp.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.OpenParen = GetExpectedToken(TokenType.OpenParen);
            node.Exp = ParseExpression().ToBuilder(); //Question... null
            node.CloseParen = GetExpectedToken(TokenType.CloseParen);
            node.Length = currentToken.End;
            return node.ToImmutable();
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = GetExpectedToken(TokenType.FunctionKeyword);
            node.FunctionBody = ParseFunctionBody().ToBuilder();
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private FuncBody ParseFunctionBody()
        {
            var node = FuncBody.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenParen = GetExpectedToken(TokenType.OpenParen);
            node.ParameterList = ParseParList().ToBuilder();
            node.CloseParen = GetExpectedToken(TokenType.CloseParen);
            node.Block = ParseBlock(Context.FuncBodyContext).ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private ParList ParseParList()
        {
            //TODO: implement;
            return null;
        }

        private ExpList ParseExpList()
        {
            contextStack.Push(Context.ExpListContext);

            var node = ExpList.CreateBuilder();
            node.StartPosition = Peek().Start;

            Expression exp = ParseExpression();
            if(exp == null)
            {
                return null;
            }

            List<ExpressionCommaPair> expressions = new List<ExpressionCommaPair>();
            expressions.Add(ExpressionCommaPair.Create(null, exp));

            while(ParseExpected(TokenType.Comma))
            {
                expressions.Add(ExpressionCommaPair.Create(currentToken, ParseExpression()));
            }

            node.Expressions = expressions.ToImmutableList();
            node.Length = currentToken.End - node.StartPosition;

            contextStack.Pop();
            return node.ToImmutable();
        }
        #endregion

        #region Helper Methods
        private bool IsContextTerminator(Context parsingContext, TokenType currentTokenType)
        {
            if (currentTokenType == TokenType.ReturnKeyword)
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

        private bool IsBinop(TokenType type)
        {
            switch (type)
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
        #endregion
    }
}