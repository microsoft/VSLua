using LanguageService.LanguageModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace LanguageService
{
    public class Parser
    {
        private Stack<ParsingContext> contextStack;
        private Token currentToken;
        private List<Token> tokenList;
        private int positionInTokenList;
        private int textPosition;
        private List<ParseError> errorList;

        public Parser()
        {
            contextStack = new Stack<ParsingContext>();
            errorList = new List<ParseError>();
            positionInTokenList = -1;
        }

        public SyntaxTree CreateSyntaxTree(TextReader luaStream)
        {
            positionInTokenList = -1;  //Make sure that internal state is at "beginning"
            tokenList = Lexer.Tokenize(luaStream);

            if (tokenList.Count == 1)
            {
                currentToken = Peek();
            }

            ChunkNode root = ParseChunkNode();
            return new SyntaxTree(root, errorList.ToImmutableList());
        }

        #region tokenList Accessors 
        private Token NextToken()
        {
            if (positionInTokenList + 1 < tokenList.Count)
            {
                currentToken = tokenList[++positionInTokenList];
                this.textPosition += currentToken.Start - currentToken.FullStart + currentToken.Length;
            }
            return currentToken;
        }

        private bool ParseExpected(SyntaxKind type)
        {
            int tempIndex = positionInTokenList + 1;
            if (tempIndex < tokenList.Count)
            {
                if (Peek().Kind == type)
                {
                    currentToken = NextToken();
                    return true;
                }
            }
            return false;
        }

        private Token GetExpectedToken(SyntaxKind kind)
        {
            if (ParseExpected(kind))
            {
                return currentToken;
            }
            else
            {
                if (kind == SyntaxKind.AssignmentOperator && IsBlockContext(contextStack.Peek()))
                {
                    ParseErrorAtCurrentPosition(ErrorMessages.InvalidStatementWithoutAssignementOperator);
                }
                else
                {
                    ParseErrorAtCurrentPosition(ErrorMessages.MissingToken + kind.ToString());
                }

                return Token.CreateMissingToken(currentToken.End);
            }
        }

        private Token Peek(int forwardAmount = 1)
        {
            if (positionInTokenList + forwardAmount < tokenList.Count)
            {
                return tokenList[positionInTokenList + forwardAmount];
            }
            else
            {
                return tokenList[tokenList.Count - 1];
            }
        }
        #endregion

        #region Parse Methods
        private ChunkNode ParseChunkNode()
        {
            var node = ChunkNode.CreateBuilder();
            node.Kind = SyntaxKind.ChunkNode;
            node.StartPosition = this.textPosition;
            node.ProgramBlock = ParseBlock(ParsingContext.ChunkNodeBlock).ToBuilder();
            node.EndOfFile = GetExpectedToken(SyntaxKind.EndOfFile);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private BlockNode ParseBlock(ParsingContext context)
        {
            contextStack.Push(context);
            var node = BlockNode.CreateBuilder();
            node.Kind = node.Kind = SyntaxKind.BlockNode;
            node.StartPosition = this.textPosition;
            bool EncounteredReturnStatement = false;
            List<StatementNode> children = new List<StatementNode>();

            while (!IsListTerminator(context, Peek().Kind))
            {
                if (IsListElementBeginner(context, Peek().Kind))
                {
                    if (EncounteredReturnStatement)
                        ParseErrorAtCurrentPosition(ErrorMessages.StatementAfterReturnStatement);

                    if (Peek().Kind == SyntaxKind.ReturnKeyword)
                        EncounteredReturnStatement = true;

                    children.Add(ParseStatement());
                }
                else
                {
                    if (AbortParsingListOrMoveToNextToken(context))
                        break;
                }
            }

            node.Statements = children.ToImmutableList();

            node.Length = this.textPosition - node.StartPosition;

            contextStack.Pop();
            return node.ToImmutable();
        }

        private StatementNode ParseStatement()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.Semicolon:
                    NextToken();
                    return SemiColonStatementNode.Create(SyntaxKind.SemiColonStatementNode, currentToken.Start, currentToken.Length, currentToken); //Question: could just use next token as first param
                case SyntaxKind.BreakKeyword:
                    return ParseBreakStatementNode();
                case SyntaxKind.GotoKeyword:
                    return ParseGoToStatementNode();
                case SyntaxKind.DoKeyword:
                    return ParseDoStatementNode();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatementNode();
                case SyntaxKind.ReturnKeyword:
                    return ParseRetStat();
                case SyntaxKind.RepeatKeyword:
                    return ParseRepeatStatementNode();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatementNode();
                case SyntaxKind.ForKeyword:
                    if (Peek(2).Kind == SyntaxKind.Identifier && tokenList[positionInTokenList + 3].Kind == SyntaxKind.AssignmentOperator)
                    {
                        return ParseSimpleForStatementNode();
                    }
                    else
                    {
                        return ParseMultipleArgForStatementNode();
                    }
                case SyntaxKind.FunctionKeyword:
                    return ParseGlobalFunctionStatementNode();
                case SyntaxKind.DoubleColon:
                    return ParseLabelStatementNode();
                case SyntaxKind.LocalKeyword:
                    if (Peek(2).Kind == SyntaxKind.FunctionKeyword)
                    {
                        return ParseLocalFunctionStatementNode();
                    }
                    else
                    {
                        return ParseLocalAssignmentStatementNode();
                    }
                case SyntaxKind.Identifier:
                    switch (Peek(2).Kind)
                    {
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.String:
                        case SyntaxKind.Colon:
                            return ParseFunctionCallStatementNode();
                        default:
                            return ParseAssignmentStatementNode();
                    }
                case SyntaxKind.OpenParen:
                    int tempPosition = positionInTokenList;
                    var prefixExp = ParseParenPrefixExp();
                    switch (Peek(2).Kind)
                    {
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.String:
                        case SyntaxKind.Colon:
                            return ParseFunctionCallStatementNode(prefixExp);
                        default:
                            positionInTokenList = tempPosition;
                            return ParseAssignmentStatementNode();
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Simple Statement Nodes

        private AssignmentStatementNode ParseAssignmentStatementNode()
        {
            var node = AssignmentStatementNode.CreateBuilder();
            node.StartPosition = this.textPosition;
            node.Kind = SyntaxKind.AssignmentStatementNode;
            node.VarList = ParseVarList().ToBuilder();
            node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.ExpList = ParseExpList().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private MultipleArgForStatementNode ParseMultipleArgForStatementNode()
        {
            var node = MultipleArgForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.MultipleArgForStatementNode;
            node.StartPosition = this.textPosition;
            node.ForKeyword = GetExpectedToken(SyntaxKind.ForKeyword);
            node.NameList = ParseNameList().ToBuilder();
            node.InKeyword = GetExpectedToken(SyntaxKind.InKeyword);
            node.ExpList = ParseExpList().ToBuilder();
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.ForStatementBlock).ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private SimpleForStatementNode ParseSimpleForStatementNode()
        {
            var node = SimpleForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.SimpleForStatementNode;
            node.StartPosition = this.textPosition;
            node.ForKeyword = GetExpectedToken(SyntaxKind.ForKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.Exp1 = ParseExpression().ToBuilder();
            node.Comma = GetExpectedToken(SyntaxKind.Comma);
            node.Exp2 = ParseExpression().ToBuilder();

            if (Peek().Kind == SyntaxKind.Comma)
            {
                node.OptionalComma = GetExpectedToken(SyntaxKind.Comma);
                node.OptionalExp3 = ParseExpression().ToBuilder();
            }

            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.ForStatementBlock).ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalAssignmentStatementNode ParseLocalAssignmentStatementNode()
        {
            var node = LocalAssignmentStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalAssignmentStatementNode;
            node.StartPosition = this.textPosition;
            node.LocalKeyword = GetExpectedToken(SyntaxKind.LocalKeyword);
            node.NameList = ParseNameList().ToBuilder();

            if (Peek().Kind == SyntaxKind.AssignmentOperator)
            {
                node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
                node.ExpList = ParseExpList().ToBuilder();
            }

            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalFunctionStatementNode ParseLocalFunctionStatementNode()
        {
            var node = LocalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalFunctionStatementNode;
            node.StartPosition = this.textPosition;
            node.LocalKeyword = GetExpectedToken(SyntaxKind.LocalKeyword);
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.FuncBody = ParseFunctionBody().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private LabelStatementNode ParseLabelStatementNode()
        {
            var node = LabelStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LabelStatementNode;
            node.StartPosition = this.textPosition;
            node.DoubleColon1 = GetExpectedToken(SyntaxKind.DoubleColon);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.DoubleColon2 = GetExpectedToken(SyntaxKind.DoubleColon);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private GlobalFunctionStatementNode ParseGlobalFunctionStatementNode()
        {
            var node = GlobalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GlobalFunctionStatementNode;
            node.StartPosition = this.textPosition;
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FuncName = ParseFuncNameNode().ToBuilder();
            node.FuncBody = ParseFunctionBody().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private RepeatStatementNode ParseRepeatStatementNode()
        {
            var node = RepeatStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.RepeatStatementNode;
            node.StartPosition = this.textPosition;
            node.RepeatKeyword = GetExpectedToken(SyntaxKind.RepeatKeyword);
            node.Block = ParseBlock(ParsingContext.RepeatStatementBlock).ToBuilder();
            node.UntilKeyword = GetExpectedToken(SyntaxKind.UntilKeyword);
            node.Exp = ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private WhileStatementNode ParseWhileStatementNode()
        {
            var node = WhileStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.WhileStatementNode;
            node.StartPosition = this.textPosition;
            node.WhileKeyword = GetExpectedToken(SyntaxKind.WhileKeyword);
            node.Exp = ParseExpression().ToBuilder();
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.WhileBlock).ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private DoStatementNode ParseDoStatementNode()
        {
            var node = DoStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.DoStatementNode;
            node.StartPosition = this.textPosition;
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.DoStatementBlock).ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private BreakStatementNode ParseBreakStatementNode()
        {
            var node = BreakStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.BreakStatementNode;
            node.StartPosition = this.textPosition;
            node.BreakKeyword = GetExpectedToken(SyntaxKind.BreakKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private GoToStatementNode ParseGoToStatementNode()
        {
            var node = GoToStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GoToStatementNode;
            node.StartPosition = this.textPosition;
            node.GoToKeyword = GetExpectedToken(SyntaxKind.GotoKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private IfStatementNode ParseIfStatementNode()
        {
            var node = IfStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.IfStatementNode;
            node.StartPosition = this.textPosition;
            node.IfKeyword = GetExpectedToken(SyntaxKind.IfKeyword);
            node.Exp = ParseExpression().ToBuilder();
            node.ThenKeyword = GetExpectedToken(SyntaxKind.ThenKeyword);
            node.IfBlock = ParseBlock(ParsingContext.IfBlock).ToBuilder();

            if (Peek().Kind == SyntaxKind.ElseIfKeyword)
            {
                node.ElseIfList = ParseElseIfList();
            }

            if (Peek().Kind == SyntaxKind.ElseKeyword)
            {
                node.ElseBlock = ParseElseBlock().ToBuilder();
            }

            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;

            return node.ToImmutable();
        }

        private ReturnStatementNode ParseRetStat()
        {
            var node = ReturnStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.ReturnStatementNode;
            node.StartPosition = this.textPosition;
            node.ReturnKeyword = GetExpectedToken(SyntaxKind.ReturnKeyword);
            node.ExpList = ParseExpList().ToBuilder();

            if (ParseExpected(SyntaxKind.Semicolon))
            {
                node.SemiColon = currentToken;
            }

            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private FunctionCallStatementNode ParseFunctionCallStatementNode(PrefixExp prefixExp = null)
        {
            var node = FunctionCallStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.FunctionCallStatementNode;
            node.StartPosition = this.textPosition;

            if (prefixExp != null)
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }
            else
            {
                switch (Peek().Kind)
                {
                    case SyntaxKind.OpenParen:
                        node.PrefixExp = ParseParenPrefixExp().ToBuilder();
                        break;
                    case SyntaxKind.Identifier:
                        switch (Peek(2).Kind)
                        {
                            case SyntaxKind.OpenParen:
                            case SyntaxKind.OpenCurlyBrace:
                            case SyntaxKind.String:
                            case SyntaxKind.Colon:
                                node.PrefixExp = ParseNameVar().ToBuilder();
                                break;
                            case SyntaxKind.OpenBracket:
                                node.PrefixExp = ParseSquareBracketVar(ParseNameVar()).ToBuilder();
                                break;
                            case SyntaxKind.Dot:
                                node.PrefixExp = ParseDotVar(ParseNameVar()).ToBuilder();
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = ParseArgs().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region If Statement Nodes

        private ElseBlockNode ParseElseBlock()
        {
            var node = ElseBlockNode.CreateBuilder();
            node.Kind = SyntaxKind.ElseBlockNode;
            node.StartPosition = this.textPosition;
            node.ElseKeyword = currentToken;
            node.Block = ParseBlock(ParsingContext.ElseBlock).ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ImmutableList<ElseIfBlockNode> ParseElseIfList()
        {
            //TODO change parse logic.
            var elseIfList = new List<ElseIfBlockNode>();

            while (ParseExpected(SyntaxKind.ElseIfKeyword))
            {
                elseIfList.Add(ParseElseIfBlock());
            }

            return elseIfList.ToImmutableList();
        }

        private ElseIfBlockNode ParseElseIfBlock()
        {
            var node = ElseIfBlockNode.CreateBuilder();
            node.Kind = SyntaxKind.ElseIfBlockNode;
            node.StartPosition = this.textPosition;
            node.ElseIfKeyword = currentToken;
            node.Exp = ParseExpression().ToBuilder();
            node.ThenKeyword = GetExpectedToken(SyntaxKind.ThenKeyword);
            node.Block = ParseBlock(ParsingContext.ElseIfBlock).ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region Expression Nodes

        private ExpressionNode ParseExpression()
        {
            ExpressionNode exp;
            int start = this.textPosition;
            if (ParseExpected(SyntaxKind.LengthUnop)
                || ParseExpected(SyntaxKind.NotUnop)
                || ParseExpected(SyntaxKind.MinusOperator)
                || ParseExpected(SyntaxKind.TildeUnOp))
            {
                var node = UnaryOperatorExpression.CreateBuilder();
                node.StartPosition = this.textPosition;
                node.UnaryOperator = currentToken;
                node.Exp = ParseExpression().ToBuilder();
                exp = node.ToImmutable();
            }
            else
            {
                switch (Peek().Kind)
                {
                    case SyntaxKind.NilKeyValue:
                    case SyntaxKind.FalseKeyValue:
                    case SyntaxKind.TrueKeyValue:
                    case SyntaxKind.Number:
                    case SyntaxKind.String:
                    case SyntaxKind.VarArgOperator:
                        NextToken();
                        exp = SimpleExpression.Create(SyntaxKind.SimpleExpression, this.currentToken.Start, this.currentToken.Length, currentToken);
                        break;
                    case SyntaxKind.FunctionKeyword:
                        exp = ParseFunctionDef();
                        break;
                    case SyntaxKind.OpenParen:
                        exp = ParseParenPrefixExp();
                        break;
                    case SyntaxKind.OpenCurlyBrace:
                        exp = ParseTableConstructorExp();
                        break;
                    case SyntaxKind.Identifier:
                        PrefixExp prefixExp = ParsePrefixExp();
                        switch (Peek().Kind)
                        {
                            case SyntaxKind.OpenBracket:
                                exp = ParseSquareBracketVar(prefixExp);
                                break;
                            case SyntaxKind.Dot:
                                exp = ParseDotVar(prefixExp);
                                break;
                            case SyntaxKind.OpenParen:
                            case SyntaxKind.Colon:
                            case SyntaxKind.OpenCurlyBrace:
                            case SyntaxKind.String:
                                exp = ParseFunctionCallPrefixp(prefixExp);
                                break;
                            default:
                                exp = prefixExp;
                                break;
                        }
                        break;
                    default:
                        ParseErrorAtCurrentPosition(ErrorMessages.IncompleteExpression);

                        var missingToken = Token.CreateMissingToken(currentToken.End);
                        return SimpleExpression.Create(SyntaxKind.MissingToken, missingToken.Start, missingToken.Length, missingToken);
                }
            }

            if (IsBinop(Peek().Kind))
            {
                var binop = NextToken();
                return BinaryOperatorExpression.Create(SyntaxKind.BinaryOperatorExpression, start, this.textPosition - start, exp, binop, ParseExpression()); //Question... this representation is slightly flawed if "operator precedence" matters... but does it matter? What scenario from a tooling perspective cares about precedence?
            }
            else
            {
                return exp;
            }
        }

        private FunctionCallPrefixexp ParseFunctionCallPrefixp(PrefixExp prefixExp = null)
        {

            var node = FunctionCallPrefixexp.CreateBuilder();
            node.Kind = SyntaxKind.FunctionCallExp;
            node.StartPosition = this.textPosition;

            if (prefixExp != null)
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }
            else
            {
                node.PrefixExp = ParsePrefixExp().ToBuilder();
                //switch (Peek().Kind)
                //{
                //    case SyntaxKind.OpenParen:
                //        node.PrefixExp = ParseParenPrefixExp().ToBuilder();
                //        break;
                //    case SyntaxKind.Identifier:
                //        switch (Peek(2).Kind)
                //        {
                //            case SyntaxKind.OpenParen:
                //            case SyntaxKind.OpenCurlyBrace:
                //            case SyntaxKind.String:
                //            case SyntaxKind.Colon:
                //                node.PrefixExp = ParseNameVar().ToBuilder();
                //                break;
                //            case SyntaxKind.OpenBracket:
                //                node.PrefixExp = ParseSquareBracketVar().ToBuilder();
                //                break;
                //            case SyntaxKind.Dot:
                //                node.PrefixExp = ParseDotVar().ToBuilder();
                //                break;
                //            default:
                //                throw new NotImplementedException();
                //        }
                //        break;
                //    default:
                //        throw new NotImplementedException();
                //}
            }

            if (ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = ParseArgs().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructorExp ParseTableConstructorExp()
        {
            var node = TableConstructorExp.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorExp;
            node.StartPosition = this.textPosition;
            node.OpenCurly = GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = ParseFieldList().ToBuilder();
            node.CloseCurly = GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #region Field Nodes

        private SeparatedList ParseFieldList()
        {
            return ParseSeperatedList(ParsingContext.FieldList);
        }

        private FieldNode ParseField()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.OpenBracket:
                    return ParseBracketField();
                case SyntaxKind.Identifier:
                    if (Peek(2).Kind == SyntaxKind.AssignmentOperator)
                    {
                        var node = AssignmentField.CreateBuilder();
                        node.Kind = SyntaxKind.AssignmentField;
                        node.StartPosition = this.textPosition;
                        node.Name = GetExpectedToken(SyntaxKind.Identifier);
                        node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
                        node.Exp = ParseExpression().ToBuilder();
                        node.Length = this.textPosition - node.StartPosition;
                        return node.ToImmutable();
                    }
                    else
                    {
                        return ParseExpField();
                    }
                default:
                    return ParseExpField();
            }
        }

        private BracketField ParseBracketField()
        {
            var node = BracketField.CreateBuilder();
            node.Kind = SyntaxKind.BracketField;
            node.StartPosition = this.textPosition;
            node.OpenBracket = GetExpectedToken(SyntaxKind.OpenBracket);
            node.IdentifierExp = ParseExpression().ToBuilder();
            node.CloseBracket = GetExpectedToken(SyntaxKind.CloseBracket);
            node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.AssignedExp = ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ExpField ParseExpField()
        {
            var node = ExpField.CreateBuilder();
            node.Kind = SyntaxKind.ExpField;
            node.StartPosition = this.textPosition;
            node.Exp = ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region PrefixExp Expression

        private PrefixExp ParsePrefixExp(PrefixExp prefixExp = null)
        {
            if (prefixExp == null)
            {
                switch (Peek().Kind)
                {
                    case SyntaxKind.Identifier:
                        prefixExp = ParseNameVar();
                        break;
                    case SyntaxKind.OpenParen:
                        prefixExp = ParseParenPrefixExp();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            while (ContinueParsingPrefixExp(Peek().Kind))
            {
                switch (Peek().Kind)
                {
                    case SyntaxKind.Dot:
                        prefixExp = ParseDotVar(prefixExp);
                        break;
                    case SyntaxKind.OpenBracket:
                        prefixExp = ParseSquareBracketVar(prefixExp);
                        break;
                    case SyntaxKind.String:
                    case SyntaxKind.OpenParen:
                    case SyntaxKind.OpenCurlyBrace:
                    case SyntaxKind.Colon:
                        prefixExp = ParseFunctionCallPrefixp(prefixExp);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return prefixExp;
        }

        private ParenPrefixExp ParseParenPrefixExp()
        {
            var node = ParenPrefixExp.CreateBuilder();
            node.Kind = SyntaxKind.ParenPrefixExp;
            node.StartPosition = this.textPosition;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.Exp = ParseExpression().ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.Kind = SyntaxKind.FunctionDef;
            node.StartPosition = this.textPosition;
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FunctionBody = ParseFunctionBody().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private SquareBracketVar ParseSquareBracketVar(PrefixExp prefixExp = null)
        {
            var node = SquareBracketVar.CreateBuilder();
            node.Kind = SyntaxKind.SquareBracketVar;
            node.StartPosition = this.textPosition;

            if (prefixExp == null)
            {
                //if (Peek().Kind == SyntaxKind.Identifier && Peek(2).Kind == SyntaxKind.OpenBracket)
                //{
                //    node.PrefixExp = ParseNameVar().ToBuilder();//TODO: test for error? this isn't correct
                //}
                //else
                //{
                node.PrefixExp = ParsePrefixExp().ToBuilder();
                //}
            }
            else
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }

            node.OpenBracket = GetExpectedToken(SyntaxKind.OpenBracket);
            node.Exp = ParseExpression().ToBuilder();
            node.CloseBracket = GetExpectedToken(SyntaxKind.CloseBracket);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private DotVar ParseDotVar(PrefixExp prefixExp = null)
        {
            var node = DotVar.CreateBuilder();
            node.Kind = SyntaxKind.DotVar;
            node.StartPosition = this.textPosition;

            if (prefixExp == null)
            {
                node.PrefixExp = ParsePrefixExp().ToBuilder(); //TODO: test for error? this isn't correct
            }
            else
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }

            node.DotOperator = GetExpectedToken(SyntaxKind.Dot);
            node.NameIdentifier = GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private Var ParseVar()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.Identifier:
                    switch (Peek(2).Kind)
                    {
                        case SyntaxKind.OpenBracket:
                            return ParseSquareBracketVar(ParseNameVar());
                        case SyntaxKind.Dot:
                            return ParseDotVar(ParseNameVar());
                        case SyntaxKind.Colon:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.String:
                            var functionCallPrefixExp = ParseFunctionCallPrefixp(ParseNameVar());
                            return ParsePotentialVarWithPrefixExp(functionCallPrefixExp);
                        default:
                            return ParseNameVar();
                    }
                case SyntaxKind.OpenParen:
                    var parenPrefixExp = ParseParenPrefixExp();
                    return ParsePotentialVarWithPrefixExp(parenPrefixExp);
                default:
                    ParseErrorAtCurrentPosition(ErrorMessages.InvalidVar);

                    var missingToken = Token.CreateMissingToken(this.textPosition);
                    return NameVar.Create(SyntaxKind.MissingToken, this.textPosition, 0, missingToken);
            }
        }

        private NameVar ParseNameVar()
        {
            var node = NameVar.CreateBuilder();
            node.Kind = SyntaxKind.NameVar;
            node.StartPosition = this.textPosition;
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #endregion

        #region Args Nodes

        private Args ParseArgs()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.OpenParen:
                    return ParseParenArg();
                case SyntaxKind.OpenCurlyBrace:
                    return ParseTableConstructorArg();
                case SyntaxKind.String:
                    return ParseStringArg();
                default:
                    ParseErrorAtCurrentPosition(ErrorMessages.InvalidArgs);

                    var missingToken = Token.CreateMissingToken(this.textPosition);
                    return StringArg.Create(SyntaxKind.MissingToken, this.textPosition, 0, missingToken);
            }
        }

        private StringArg ParseStringArg()
        {
            var node = StringArg.CreateBuilder();
            node.Kind = SyntaxKind.StringArg;
            node.StartPosition = this.textPosition;
            node.StringLiteral = GetExpectedToken(SyntaxKind.String);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private TableContructorArg ParseTableConstructorArg()
        {
            var node = TableContructorArg.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorArg;
            node.StartPosition = this.textPosition;
            node.OpenCurly = GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = ParseFieldList().ToBuilder();
            node.CloseCurly = GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ParenArg ParseParenArg()
        {
            var node = ParenArg.CreateBuilder();
            node.Kind = SyntaxKind.ParenArg;
            node.StartPosition = this.textPosition;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.ExpList = ParseExpList().ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region List Nodes

        private SeparatedList ParseSeperatedList(ParsingContext context)
        {
            contextStack.Push(context);
            var listNode = SeparatedList.CreateBuilder();
            var syntaxList = new List<SeparatedListElement>();
            listNode.Kind = GetListKind(context);
            listNode.StartPosition = this.textPosition;
            bool commaFound = false;

            while (true)
            {
                if (IsListElementBeginner(context, Peek().Kind))
                {
                    var node = SeparatedListElement.CreateBuilder();

                    node.StartPosition = this.textPosition;
                    node.Kind = SyntaxKind.SeparatedListElement;
                    node.Element = ParseListElement(context);

                    if (IsValidSeparator(context, Peek().Kind))
                    {
                        node.Seperator = NextToken();
                        commaFound = true;
                    }
                    else
                    {
                        commaFound = false;
                    }

                    node.Length = currentToken.End - node.StartPosition;

                    syntaxList.Add(node.ToImmutable());

                    if (commaFound)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (IsListTerminator(context, Peek().Kind))
                    break;

                if (AbortParsingListOrMoveToNextToken(context))
                    break;
            }

            listNode.SyntaxList = syntaxList.ToImmutableList();

            listNode.Length = this.textPosition - listNode.StartPosition;

            if (contextStack.Count > 0 && contextStack.Peek() == context)
            {
                contextStack.Pop();
            }

            return listNode.ToImmutable();
        }

        private bool IsValidSeparator(ParsingContext context, SyntaxKind seperatorKind)
        {
            switch (context)
            {
                case ParsingContext.ExpList:
                case ParsingContext.NameList:
                case ParsingContext.VarList:
                    return seperatorKind == SyntaxKind.Comma;
                case ParsingContext.FieldList:
                    return (seperatorKind == SyntaxKind.Comma || seperatorKind == SyntaxKind.Semicolon);
                case ParsingContext.FuncNameDotSeperatedNameList:
                    return seperatorKind == SyntaxKind.Dot;
                default:
                    throw new InvalidOperationException();
            }
        }

        private SyntaxNodeOrToken ParseListElement(ParsingContext context)
        {
            switch (context)
            {
                case ParsingContext.ExpList:
                    return ParseExpression();
                case ParsingContext.NameList:
                case ParsingContext.FuncNameDotSeperatedNameList:
                    if (ParseExpected(SyntaxKind.Identifier))
                    {
                        return currentToken;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                case ParsingContext.VarList:
                    return ParseVar();
                case ParsingContext.FieldList:
                    return ParseField();
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool IsListTerminator(ParsingContext context, SyntaxKind tokenType)
        {
            if (tokenType == SyntaxKind.EndOfFile || tokenType == SyntaxKind.EndKeyword)
                return true;

            switch (context)
            {
                case ParsingContext.IfBlock:
                    return (tokenType == SyntaxKind.EndKeyword || tokenType == SyntaxKind.ElseIfKeyword || tokenType == SyntaxKind.ElseKeyword);
                case ParsingContext.ElseBlock:
                    return (tokenType == SyntaxKind.EndKeyword);
                case ParsingContext.ElseIfBlock:
                    return (tokenType == SyntaxKind.ElseIfKeyword || tokenType == SyntaxKind.EndKeyword);
                case ParsingContext.ChunkNodeBlock:
                    return tokenType == SyntaxKind.EndOfFile;
                case ParsingContext.FuncBodyBlock:
                case ParsingContext.WhileBlock:
                case ParsingContext.DoStatementBlock:
                case ParsingContext.ForStatementBlock:
                    return tokenType == SyntaxKind.EndKeyword;
                case ParsingContext.RepeatStatementBlock:
                    return tokenType == SyntaxKind.UntilKeyword;
                case ParsingContext.ExpList:
                    return tokenType == SyntaxKind.CloseParen || tokenType == SyntaxKind.Semicolon || tokenType == SyntaxKind.DoKeyword; //TODO: whatabout end of assignment statement context?
                case ParsingContext.NameList:
                    return tokenType == SyntaxKind.InKeyword || tokenType == SyntaxKind.CloseParen || tokenType == SyntaxKind.AssignmentOperator; //TODO: whatabout end of assignment statement context?
                case ParsingContext.FuncNameDotSeperatedNameList:
                    return tokenType == SyntaxKind.Colon || tokenType == SyntaxKind.OpenParen; //TODO: Confirm there is no concretely defined terminator...
                case ParsingContext.VarList:
                    return tokenType == SyntaxKind.AssignmentOperator;
                case ParsingContext.FieldList:
                    return tokenType == SyntaxKind.CloseCurlyBrace;
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool IsListElementBeginner(ParsingContext context, SyntaxKind tokenType)
        {
            switch (context)
            {
                case ParsingContext.IfBlock:
                case ParsingContext.ChunkNodeBlock:
                case ParsingContext.ElseBlock:
                case ParsingContext.ElseIfBlock:
                case ParsingContext.DoStatementBlock:
                case ParsingContext.FuncBodyBlock:
                case ParsingContext.WhileBlock:
                case ParsingContext.RepeatStatementBlock:
                case ParsingContext.ForStatementBlock:
                    switch (tokenType)
                    {
                        case SyntaxKind.Identifier:
                        case SyntaxKind.Semicolon:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.DoubleColon:
                        case SyntaxKind.BreakKeyword:
                        case SyntaxKind.GotoKeyword:
                        case SyntaxKind.DoKeyword:
                        case SyntaxKind.WhileKeyword:
                        case SyntaxKind.RepeatKeyword:
                        case SyntaxKind.IfKeyword:
                        case SyntaxKind.ForKeyword:
                        case SyntaxKind.FunctionKeyword:
                        case SyntaxKind.LocalKeyword:
                        case SyntaxKind.ReturnKeyword:
                            return true;
                        default:
                            return false;
                    }
                case ParsingContext.ExpList:
                    switch (tokenType)
                    {
                        case SyntaxKind.NilKeyValue:
                        case SyntaxKind.FalseKeyValue:
                        case SyntaxKind.TrueKeyValue:
                        case SyntaxKind.Number:
                        case SyntaxKind.String:
                        case SyntaxKind.VarArgOperator:
                        case SyntaxKind.FunctionKeyword:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.Identifier:
                            return true;
                        default:
                            return false;
                    }
                case ParsingContext.FieldList:
                    switch (tokenType)
                    {
                        case SyntaxKind.NilKeyValue:
                        case SyntaxKind.OpenBracket:
                        case SyntaxKind.FalseKeyValue:
                        case SyntaxKind.TrueKeyValue:
                        case SyntaxKind.Number:
                        case SyntaxKind.String:
                        case SyntaxKind.VarArgOperator:
                        case SyntaxKind.FunctionKeyword:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.Identifier:
                            return true;
                        default:
                            return false;
                    }
                case ParsingContext.NameList:
                case ParsingContext.FuncNameDotSeperatedNameList:
                    if (tokenType == SyntaxKind.Identifier)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ParsingContext.VarList:
                    if (tokenType == SyntaxKind.Identifier || tokenType == SyntaxKind.OpenParen)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        private SyntaxKind GetListKind(ParsingContext context)
        {
            switch (context)
            {
                case ParsingContext.ExpList:
                    return SyntaxKind.ExpList;
                case ParsingContext.NameList:
                    return SyntaxKind.NameList;
                case ParsingContext.VarList:
                    return SyntaxKind.VarList;
                case ParsingContext.FieldList:
                    return SyntaxKind.FieldList;
                case ParsingContext.FuncNameDotSeperatedNameList:
                    return SyntaxKind.DotSeparatedNameList;
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Code To Deprecate
        private SeparatedList ParseVarList()
        {
            return ParseSeperatedList(ParsingContext.VarList);
        }

        private ParList ParseParList()
        {
            if (Peek().Kind == SyntaxKind.VarArgOperator)
            {
                var node = VarArgParList.CreateBuilder();
                node.Kind = SyntaxKind.VarArgPar;
                node.StartPosition = this.textPosition;
                node.VarargOperator = GetExpectedToken(SyntaxKind.VarArgOperator);
                node.Length = this.textPosition - node.StartPosition;
                return node.ToImmutable();
            }
            else
            {
                var node = NameListPar.CreateBuilder();
                node.Kind = SyntaxKind.NameListPar;
                node.StartPosition = this.textPosition;
                node.NamesList = ParseNameList().ToBuilder();
                if (ParseExpected(SyntaxKind.Comma))
                {
                    node.Comma = currentToken;
                    node.Vararg = GetExpectedToken(SyntaxKind.VarArgOperator);
                }
                node.Length = this.textPosition - node.StartPosition;
                return node.ToImmutable();
            }
        }

        private SeparatedList ParseNameList()
        {
            return ParseSeperatedList(ParsingContext.NameList);
        }

        private SeparatedList ParseExpList()
        {
            return ParseSeperatedList(ParsingContext.ExpList);
        }

        #endregion

        #endregion

        private FuncBodyNode ParseFunctionBody()
        {
            var node = FuncBodyNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncBodyNode;
            node.StartPosition = this.textPosition;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.ParameterList = ParseParList().ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Block = ParseBlock(ParsingContext.FuncBodyBlock).ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private FuncNameNode ParseFuncNameNode()
        {
            var node = FuncNameNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncNameNode;
            node.StartPosition = this.textPosition;
            node.Name = GetExpectedToken(SyntaxKind.Identifier);

            node.FuncNameList = ParseSeperatedList(ParsingContext.FuncNameDotSeperatedNameList).ToBuilder();
            if (ParseExpected(SyntaxKind.Colon))
            {
                node.OptionalColon = currentToken;
                node.OptionalName = GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region Error Methods

        private string GetContextError(ParsingContext context)
        {
            switch (context)
            {
                case ParsingContext.IfBlock:
                case ParsingContext.ChunkNodeBlock:
                case ParsingContext.ElseBlock:
                case ParsingContext.ElseIfBlock:
                case ParsingContext.DoStatementBlock:
                case ParsingContext.FuncBodyBlock:
                case ParsingContext.WhileBlock:
                case ParsingContext.RepeatStatementBlock:
                case ParsingContext.ForStatementBlock:
                    return ErrorMessages.BlockParseError;
                case ParsingContext.VarList:
                    return ErrorMessages.VarListParseError;
                case ParsingContext.ExpList:
                    return ErrorMessages.ExpListParseError;
                case ParsingContext.NameList:
                    return ErrorMessages.NameListParseError;
                case ParsingContext.FieldList:
                    return ErrorMessages.FieldListParseError;
                case ParsingContext.FuncNameDotSeperatedNameList:
                    return ErrorMessages.FuncNameParseError;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ParseErrorAtCurrentToken(string message)
        {
            if (positionInTokenList == -1)
            {
                errorList.Add(new ParseError(message, 0, 0));
            }
            else
            {
                errorList.Add(new ParseError(message, currentToken.Start, currentToken.End));
            }
        }

        private void ParseErrorAtCurrentPosition(string message)
        {
            errorList.Add(new ParseError(message, Peek().FullStart, Peek().FullStart));
        }

        private bool AbortParsingListOrMoveToNextToken(ParsingContext context)
        {
            ParseErrorAtCurrentToken(GetContextError(context));
            if (isInSomeParsingContext())
            {
                return true;
            }

            SkipCurrentToken();
            return false;
        }

        private void SkipCurrentToken(string message = null)
        {
            //TODO: conduct roundtrip test on erroneous files
            NextToken();

            var tempTriviaList = currentToken.LeadingTrivia;
            tempTriviaList.Add(new Trivia(currentToken.Kind, currentToken.Text));

            foreach (var triv in Peek().LeadingTrivia)
            {
                tempTriviaList.Add(triv);
            }

            var tokenWithAddedTrivia = new Token(Peek().Kind, Peek().Text, tempTriviaList, currentToken.FullStart, this.textPosition);

            tokenList.RemoveAt(positionInTokenList);

            if (positionInTokenList > tokenList.Count)
            {
                tokenList.Add(tokenWithAddedTrivia);
            }
            else
            {
                tokenList[positionInTokenList] = tokenWithAddedTrivia;
            }

            positionInTokenList--;
            currentToken = (positionInTokenList >= 0) ? currentToken = tokenList[positionInTokenList] : null;
        }

        private bool isInSomeParsingContext()
        {
            var tempStack = new Stack<ParsingContext>(contextStack.Reverse());

            while (contextStack.Count > 0)
            {
                if (IsListElementBeginner(contextStack.Peek(), Peek().Kind))
                {
                    return true;
                }
                else
                {
                    contextStack.Pop();
                }
            }

            contextStack = tempStack;
            return false;
        }

        #endregion

        #region Helper Methods

        private bool IsBinop(SyntaxKind type)
        {
            switch (type)
            {
                case SyntaxKind.PlusOperator:
                case SyntaxKind.MinusOperator:
                case SyntaxKind.MultiplyOperator:
                case SyntaxKind.DivideOperator:
                case SyntaxKind.FloorDivideOperator:
                case SyntaxKind.ExponentOperator:
                case SyntaxKind.ModulusOperator:
                case SyntaxKind.TildeUnOp: //TODO: deal with ambiguity
                case SyntaxKind.BitwiseAndOperator:
                case SyntaxKind.BitwiseOrOperator:
                case SyntaxKind.BitwiseRightOperator:
                case SyntaxKind.BitwiseLeftOperator:
                case SyntaxKind.NotEqualsOperator:
                case SyntaxKind.LessOrEqualOperator:
                case SyntaxKind.GreaterOrEqualOperator:
                case SyntaxKind.EqualityOperator:
                case SyntaxKind.StringConcatOperator:
                case SyntaxKind.GreaterThanOperator:
                case SyntaxKind.LessThanOperator:
                case SyntaxKind.AndBinop:
                case SyntaxKind.OrBinop:
                    return true;
                default:
                    return false;
            }
        }

        private bool ContinueParsingPrefixExp(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Dot:
                case SyntaxKind.OpenBracket:
                case SyntaxKind.String:
                case SyntaxKind.Colon:
                case SyntaxKind.OpenParen:
                case SyntaxKind.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }

        private Var ParsePotentialVarWithPrefixExp(PrefixExp prefixExp)
        {
            if (Peek().Kind == SyntaxKind.OpenBracket)
            {
                return ParseSquareBracketVar(prefixExp);
            }
            else
            {
                //Arbitrarily assuming it's a dot var as that is the only valid alternative
                //when there is no open bracket... will log errors, if the requirements for dot var are not found
                return ParseDotVar(prefixExp);
            }
        }

        private bool IsBlockContext(ParsingContext context)
        {
            switch (context)
            {
                case ParsingContext.IfBlock:
                case ParsingContext.ChunkNodeBlock:
                case ParsingContext.ElseBlock:
                case ParsingContext.ElseIfBlock:
                case ParsingContext.DoStatementBlock:
                case ParsingContext.FuncBodyBlock:
                case ParsingContext.WhileBlock:
                case ParsingContext.RepeatStatementBlock:
                case ParsingContext.ForStatementBlock:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}