using LanguageService.LanguageModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Internal.VisualStudio.Shell;

namespace LanguageService
{
    internal class Parser
    {
        private Stack<ParsingContext> contextStack;
        private Token currentToken;
        private List<Token> tokenList;
        private List<StatementNode> statementNodeList;
        private int positionInTokenList;
        private int textPosition;
        private List<ParseError> errorList;

        private Parser()
        {
            this.contextStack = new Stack<ParsingContext>();
            this.errorList = new List<ParseError>();
            this.positionInTokenList = -1;
        }

        internal static SyntaxTree Parse(TextReader luaReader)
        {
            return new Parser().CreateSyntaxTreeInner(luaReader);
        }

        private  SyntaxTree CreateSyntaxTreeInner(TextReader luaReader)
        {
            Validate.IsNotNull(luaReader, nameof(luaReader));

            this.positionInTokenList = -1;  //Make sure that internal state is at "beginning"
            this.tokenList = Lexer.Tokenize(luaReader);

            if (this.tokenList.Count == 1)
            {
                this.currentToken = this.Peek();
            }

            this.statementNodeList = new List<StatementNode>();
            ChunkNode root = this.ParseChunkNode();
            return new SyntaxTree(root, this.tokenList, this.statementNodeList, this.errorList.ToImmutableList());
        }

        #region tokenList Accessors
        private Token NextToken()
        {
            if (this.positionInTokenList + 1 < this.tokenList.Count)
            {
                this.currentToken = this.tokenList[++this.positionInTokenList];
                this.textPosition += this.currentToken.Start - this.currentToken.FullStart + this.currentToken.Length;
            }
            return this.currentToken;
        }

        private void SetPositionInTokenList(int position)
        {
            this.positionInTokenList = position;
            this.textPosition = position >= 0 ? this.tokenList[position].End : 0;
        }

        private bool ParseExpected(SyntaxKind type)
        {
            int tempIndex = this.positionInTokenList + 1;
            if (tempIndex < this.tokenList.Count)
            {
                if (this.Peek().Kind == type)
                {
                    this.currentToken = this.NextToken();
                    return true;
                }
            }
            return false;
        }

        private Token GetExpectedToken(SyntaxKind kind)
        {
            if (this.ParseExpected(kind))
            {
                return this.currentToken;
            }
            else if (kind == SyntaxKind.AssignmentOperator && this.IsBlockContext(this.contextStack.Peek()))
            {
                this.ParseErrorAtCurrentPosition(ErrorMessages.InvalidStatementWithoutAssignementOperator);
            }
            else
            {
                this.ParseErrorAtCurrentPosition(ErrorMessages.MissingToken + kind.ToString());
            }

            return Token.CreateMissingToken(this.currentToken.End);
        }

        private Token Peek(int forwardAmount = 1)
        {
            if (this.positionInTokenList + forwardAmount < this.tokenList.Count)
            {
                return this.tokenList[this.positionInTokenList + forwardAmount];
            }
            else
            {
                return this.tokenList[this.tokenList.Count - 1];
            }
        }
        #endregion

        #region Parse Methods
        private ChunkNode ParseChunkNode()
        {
            var node = ChunkNode.CreateBuilder();
            node.Kind = SyntaxKind.ChunkNode;
            node.StartPosition = this.textPosition;
            node.ProgramBlock = this.ParseBlock(ParsingContext.ChunkNodeBlock).ToBuilder();
            node.EndOfFile = this.GetExpectedToken(SyntaxKind.EndOfFile);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private BlockNode ParseBlock(ParsingContext context)
        {
            this.contextStack.Push(context);
            var node = BlockNode.CreateBuilder();
            node.Kind = node.Kind = SyntaxKind.BlockNode;
            node.StartPosition = this.textPosition;
            bool EncounteredReturnStatement = false;
            var children = ImmutableList.CreateBuilder<StatementNode>();

            while (!this.IsListTerminator(context, this.Peek().Kind))
            {
                if (this.IsListElementBeginner(context, this.Peek().Kind))
                {
                    if (EncounteredReturnStatement)
                    {
                        this.ParseErrorAtCurrentPosition(ErrorMessages.StatementAfterReturnStatement);
                    }

                    if (this.Peek().Kind == SyntaxKind.ReturnKeyword)
                    {
                        EncounteredReturnStatement = true;
                    }

                    children.Add(this.ParseStatement());
                }
                else
                {
                    if (this.AbortParsingListOrMoveToNextToken(context))
                    {
                        break;
                    }
                }
            }

            node.Statements = children.ToImmutableList();

            node.Length = this.textPosition - node.StartPosition;

            this.contextStack.Pop();
            return node.ToImmutable();
        }

        private StatementNode ParseStatement()
        {
            switch (this.Peek().Kind)
            {
                case SyntaxKind.Semicolon:
                    this.NextToken();
                    return SemiColonStatementNode.Create(SyntaxKind.SemiColonStatementNode, this.currentToken.Start, this.currentToken.Length, this.currentToken); //Question: could just use next token as first param
                case SyntaxKind.BreakKeyword:
                    return this.ParseBreakStatementNode();
                case SyntaxKind.GotoKeyword:
                    return this.ParseGoToStatementNode();
                case SyntaxKind.DoKeyword:
                    return this.ParseDoStatementNode();
                case SyntaxKind.WhileKeyword:
                    return this.ParseWhileStatementNode();
                case SyntaxKind.ReturnKeyword:
                    return this.ParseRetStat();
                case SyntaxKind.RepeatKeyword:
                    return this.ParseRepeatStatementNode();
                case SyntaxKind.IfKeyword:
                    return this.ParseIfStatementNode();
                case SyntaxKind.ForKeyword:
                    if (this.Peek(2).Kind == SyntaxKind.Identifier && this.tokenList[this.positionInTokenList + 3].Kind == SyntaxKind.AssignmentOperator)
                    {
                        return this.ParseSimpleForStatementNode();
                    }
                    else
                    {
                        return this.ParseMultipleArgForStatementNode();
                    }
                case SyntaxKind.FunctionKeyword:
                    return this.ParseGlobalFunctionStatementNode();
                case SyntaxKind.DoubleColon:
                    return this.ParseLabelStatementNode();
                case SyntaxKind.LocalKeyword:
                    if (this.Peek(2).Kind == SyntaxKind.FunctionKeyword)
                    {
                        return this.ParseLocalFunctionStatementNode();
                    }
                    else
                    {
                        return this.ParseLocalAssignmentStatementNode();
                    }
                case SyntaxKind.Identifier:
                case SyntaxKind.OpenParen:
                    int tempPosition = this.positionInTokenList;
                    var prefixExp = this.ParsePrefixExp();
                    this.SetPositionInTokenList(tempPosition);

                    if (prefixExp is FunctionCallPrefixexp)
                    {
                        return this.ParseFunctionCallStatementNode();
                    }
                    else
                    {
                        return this.ParseAssignmentStatementNode();
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
            node.VarList = this.ParseVarList().ToBuilder();
            node.AssignmentOperator = this.GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.ExpList = this.ParseExpList().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            this.statementNodeList.Add(node.ToImmutable());
            return node.ToImmutable();
        }

        private MultipleArgForStatementNode ParseMultipleArgForStatementNode()
        {
            var node = MultipleArgForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.MultipleArgForStatementNode;
            node.StartPosition = this.textPosition;
            node.ForKeyword = this.GetExpectedToken(SyntaxKind.ForKeyword);
            node.NameList = this.ParseNameList().ToBuilder();
            node.InKeyword = this.GetExpectedToken(SyntaxKind.InKeyword);
            node.ExpList = this.ParseExpList().ToBuilder();
            node.DoKeyword = this.GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = this.ParseBlock(ParsingContext.ForStatementBlock).ToBuilder();
            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private SimpleForStatementNode ParseSimpleForStatementNode()
        {
            var node = SimpleForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.SimpleForStatementNode;
            node.StartPosition = this.textPosition;
            node.ForKeyword = this.GetExpectedToken(SyntaxKind.ForKeyword);
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            node.AssignmentOperator = this.GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.Exp1 = this.ParseExpression().ToBuilder();
            node.Comma = this.GetExpectedToken(SyntaxKind.Comma);
            node.Exp2 = this.ParseExpression().ToBuilder();

            if (this.Peek().Kind == SyntaxKind.Comma)
            {
                node.OptionalComma = this.GetExpectedToken(SyntaxKind.Comma);
                node.OptionalExp3 = this.ParseExpression().ToBuilder();
            }

            node.DoKeyword = this.GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = this.ParseBlock(ParsingContext.ForStatementBlock).ToBuilder();
            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalAssignmentStatementNode ParseLocalAssignmentStatementNode()
        {
            var node = LocalAssignmentStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalAssignmentStatementNode;
            node.StartPosition = this.textPosition;
            node.LocalKeyword = this.GetExpectedToken(SyntaxKind.LocalKeyword);
            node.NameList = this.ParseNameList().ToBuilder();

            if (this.Peek().Kind == SyntaxKind.AssignmentOperator)
            {
                node.AssignmentOperator = this.GetExpectedToken(SyntaxKind.AssignmentOperator);
                node.ExpList = this.ParseExpList().ToBuilder();
            }

            node.Length = this.textPosition - node.StartPosition;
            this.statementNodeList.Add(node.ToImmutable());
            return node.ToImmutable();
        }

        private LocalFunctionStatementNode ParseLocalFunctionStatementNode()
        {
            var node = LocalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalFunctionStatementNode;
            node.StartPosition = this.textPosition;
            node.LocalKeyword = this.GetExpectedToken(SyntaxKind.LocalKeyword);
            node.FunctionKeyword = this.GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            node.FuncBody = this.ParseFunctionBody().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private LabelStatementNode ParseLabelStatementNode()
        {
            var node = LabelStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LabelStatementNode;
            node.StartPosition = this.textPosition;
            node.DoubleColon1 = this.GetExpectedToken(SyntaxKind.DoubleColon);
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            node.DoubleColon2 = this.GetExpectedToken(SyntaxKind.DoubleColon);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private GlobalFunctionStatementNode ParseGlobalFunctionStatementNode()
        {
            var node = GlobalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GlobalFunctionStatementNode;
            node.StartPosition = this.textPosition;
            node.FunctionKeyword = this.GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FuncName = this.ParseFuncNameNode().ToBuilder();
            node.FuncBody = this.ParseFunctionBody().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private RepeatStatementNode ParseRepeatStatementNode()
        {
            var node = RepeatStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.RepeatStatementNode;
            node.StartPosition = this.textPosition;
            node.RepeatKeyword = this.GetExpectedToken(SyntaxKind.RepeatKeyword);
            node.Block = this.ParseBlock(ParsingContext.RepeatStatementBlock).ToBuilder();
            node.UntilKeyword = this.GetExpectedToken(SyntaxKind.UntilKeyword);
            node.Exp = this.ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private WhileStatementNode ParseWhileStatementNode()
        {
            var node = WhileStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.WhileStatementNode;
            node.StartPosition = this.textPosition;
            node.WhileKeyword = this.GetExpectedToken(SyntaxKind.WhileKeyword);
            node.Exp = this.ParseExpression().ToBuilder();
            node.DoKeyword = this.GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = this.ParseBlock(ParsingContext.WhileBlock).ToBuilder();
            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private DoStatementNode ParseDoStatementNode()
        {
            var node = DoStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.DoStatementNode;
            node.StartPosition = this.textPosition;
            node.DoKeyword = this.GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = this.ParseBlock(ParsingContext.DoStatementBlock).ToBuilder();
            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private BreakStatementNode ParseBreakStatementNode()
        {
            var node = BreakStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.BreakStatementNode;
            node.StartPosition = this.textPosition;
            node.BreakKeyword = this.GetExpectedToken(SyntaxKind.BreakKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private GoToStatementNode ParseGoToStatementNode()
        {
            var node = GoToStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GoToStatementNode;
            node.StartPosition = this.textPosition;
            node.GoToKeyword = this.GetExpectedToken(SyntaxKind.GotoKeyword);
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private IfStatementNode ParseIfStatementNode()
        {
            var node = IfStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.IfStatementNode;
            node.StartPosition = this.textPosition;
            node.IfKeyword = this.GetExpectedToken(SyntaxKind.IfKeyword);
            node.Exp = this.ParseExpression().ToBuilder();
            node.ThenKeyword = this.GetExpectedToken(SyntaxKind.ThenKeyword);
            node.IfBlock = this.ParseBlock(ParsingContext.IfBlock).ToBuilder();

            if (this.Peek().Kind == SyntaxKind.ElseIfKeyword)
            {
                node.ElseIfList = this.ParseElseIfList();
            }

            if (this.Peek().Kind == SyntaxKind.ElseKeyword)
            {
                node.ElseBlock = this.ParseElseBlock().ToBuilder();
            }

            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ReturnStatementNode ParseRetStat()
        {
            var node = ReturnStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.ReturnStatementNode;
            node.StartPosition = this.textPosition;
            node.ReturnKeyword = this.GetExpectedToken(SyntaxKind.ReturnKeyword);
            node.ExpList = this.ParseExpList().ToBuilder();

            if (this.ParseExpected(SyntaxKind.Semicolon))
            {
                node.SemiColon = this.currentToken;
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
                node.PrefixExp = this.ParsePrefixExp(null, true).ToBuilder();
            }

            if (this.ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = this.currentToken;
                node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = this.ParseArgs().ToBuilder();
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

            if (this.ParseExpected(SyntaxKind.ElseKeyword))
            {
                node.ElseKeyword = this.currentToken;
            }

            node.Block = this.ParseBlock(ParsingContext.ElseBlock).ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ImmutableList<ElseIfBlockNode> ParseElseIfList()
        {
            //TODO change parse logic.
            var elseIfList = ImmutableList.CreateBuilder<ElseIfBlockNode>();

            while (this.ParseExpected(SyntaxKind.ElseIfKeyword))
            {
                elseIfList.Add(this.ParseElseIfBlock());
            }

            return elseIfList.ToImmutableList();
        }

        private ElseIfBlockNode ParseElseIfBlock()
        {
            var node = ElseIfBlockNode.CreateBuilder();
            node.Kind = SyntaxKind.ElseIfBlockNode;
            node.StartPosition = this.textPosition;
            node.ElseIfKeyword = this.currentToken;
            node.Exp = this.ParseExpression().ToBuilder();
            node.ThenKeyword = this.GetExpectedToken(SyntaxKind.ThenKeyword);
            node.Block = this.ParseBlock(ParsingContext.ElseIfBlock).ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region Expression Nodes

        private ExpressionNode ParseExpression()
        {
            ExpressionNode exp;
            int start = this.textPosition;
            if (this.ParseExpected(SyntaxKind.LengthUnop)
                || this.ParseExpected(SyntaxKind.NotUnop)
                || this.ParseExpected(SyntaxKind.MinusOperator)
                || this.ParseExpected(SyntaxKind.TildeUnOp))
            {
                var node = UnaryOperatorExpression.CreateBuilder();
                node.StartPosition = this.textPosition;
                node.UnaryOperator = this.currentToken;
                node.Exp = this.ParseExpression().ToBuilder();
                exp = node.ToImmutable();
            }
            else
            {
                switch (this.Peek().Kind)
                {
                    case SyntaxKind.NilKeyValue:
                    case SyntaxKind.FalseKeyValue:
                    case SyntaxKind.TrueKeyValue:
                    case SyntaxKind.Number:
                    case SyntaxKind.String:
                    case SyntaxKind.VarArgOperator:
                        this.NextToken();
                        exp = SimpleExpression.Create(SyntaxKind.SimpleExpression, this.currentToken.Start, this.currentToken.Length, this.currentToken);
                        break;
                    case SyntaxKind.FunctionKeyword:
                        exp = this.ParseFunctionDef();
                        break;
                    case SyntaxKind.OpenParen:
                        exp = this.ParseParenPrefixExp();
                        break;
                    case SyntaxKind.OpenCurlyBrace:
                        exp = this.ParseTableConstructorExp();
                        break;
                    case SyntaxKind.Identifier:
                        PrefixExp prefixExp = this.ParsePrefixExp();
                        switch (this.Peek().Kind)
                        {
                            case SyntaxKind.OpenBracket:
                                exp = this.ParseSquareBracketVar(prefixExp);
                                break;
                            case SyntaxKind.Dot:
                                exp = this.ParseDotVar(prefixExp);
                                break;
                            case SyntaxKind.OpenParen:
                            case SyntaxKind.Colon:
                            case SyntaxKind.OpenCurlyBrace:
                            case SyntaxKind.String:
                                exp = this.ParseFunctionCallPrefixp(prefixExp);
                                break;
                            default:
                                exp = prefixExp;
                                break;
                        }
                        break;
                    default:
                        this.ParseErrorAtCurrentPosition(ErrorMessages.IncompleteExpression);

                        var missingToken = Token.CreateMissingToken(this.currentToken.End);
                        return SimpleExpression.Create(SyntaxKind.MissingToken, missingToken.Start, missingToken.Length, missingToken);
                }
            }

            if (this.IsBinop(this.Peek().Kind))
            {
                var binop = this.NextToken();
                return BinaryOperatorExpression.Create(SyntaxKind.BinaryOperatorExpression, start, this.textPosition - start, exp, binop, this.ParseExpression()); //Question... this representation is slightly flawed if "operator precedence" matters... but does it matter? What scenario from a tooling perspective cares about precedence?
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
                node.PrefixExp = this.ParsePrefixExp().ToBuilder();
            }

            if (this.ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = this.currentToken;
                node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = this.ParseArgs().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructorExp ParseTableConstructorExp()
        {
            var node = TableConstructorExp.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorExp;
            node.StartPosition = this.textPosition;
            node.OpenCurly = this.GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = this.ParseFieldList().ToBuilder();
            node.CloseCurly = this.GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #region Field Nodes

        private SeparatedList ParseFieldList()
        {
            return this.ParseSeparatedList(ParsingContext.FieldList);
        }

        private FieldNode ParseField()
        {
            switch (this.Peek().Kind)
            {
                case SyntaxKind.OpenBracket:
                    return this.ParseBracketField();
                case SyntaxKind.Identifier:
                    if (this.Peek(2).Kind == SyntaxKind.AssignmentOperator)
                    {
                        var node = AssignmentField.CreateBuilder();
                        node.Kind = SyntaxKind.AssignmentField;
                        node.StartPosition = this.textPosition;
                        node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
                        node.AssignmentOperator = this.GetExpectedToken(SyntaxKind.AssignmentOperator);
                        node.Exp = this.ParseExpression().ToBuilder();
                        node.Length = this.textPosition - node.StartPosition;
                        return node.ToImmutable();
                    }
                    else
                    {
                        return this.ParseExpField();
                    }
                default:
                    return this.ParseExpField();
            }
        }

        private BracketField ParseBracketField()
        {
            var node = BracketField.CreateBuilder();
            node.Kind = SyntaxKind.BracketField;
            node.StartPosition = this.textPosition;
            node.OpenBracket = this.GetExpectedToken(SyntaxKind.OpenBracket);
            node.IdentifierExp = this.ParseExpression().ToBuilder();
            node.CloseBracket = this.GetExpectedToken(SyntaxKind.CloseBracket);
            node.AssignmentOperator = this.GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.AssignedExp = this.ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ExpField ParseExpField()
        {
            var node = ExpField.CreateBuilder();
            node.Kind = SyntaxKind.ExpField;
            node.StartPosition = this.textPosition;
            node.Exp = this.ParseExpression().ToBuilder();
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region PrefixExp Expression

        private PrefixExp ParsePrefixExp(PrefixExp prefixExp = null, bool parsingFunctionCallStatement = false)
        {
            if (prefixExp == null)
            {
                switch (this.Peek().Kind)
                {
                    case SyntaxKind.Identifier:
                        prefixExp = this.ParseNameVar();
                        break;
                    case SyntaxKind.OpenParen:
                        prefixExp = this.ParseParenPrefixExp();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            int tempPosition = 0;

            while (this.ContinueParsingPrefixExp(this.Peek().Kind))
            {
                switch (this.Peek().Kind)
                {
                    case SyntaxKind.Dot:
                        prefixExp = this.ParseDotVar(prefixExp);
                        break;
                    case SyntaxKind.OpenBracket:
                        prefixExp = this.ParseSquareBracketVar(prefixExp);
                        break;
                    case SyntaxKind.String:
                    case SyntaxKind.OpenParen:
                    case SyntaxKind.OpenCurlyBrace:
                    case SyntaxKind.Colon:
                        tempPosition = this.positionInTokenList;
                        prefixExp = this.ParseFunctionCallPrefixp(prefixExp);
                        break;
                    default:
                        return prefixExp;
                }
            }

            if (parsingFunctionCallStatement)
            {
                if (prefixExp is FunctionCallPrefixexp)
                {
                    this.SetPositionInTokenList(tempPosition);
                    return (prefixExp as FunctionCallPrefixexp).PrefixExp;
                }
                else
                {
                    return prefixExp;
                }
            }

            return prefixExp;
        }

        private ParenPrefixExp ParseParenPrefixExp()
        {
            var node = ParenPrefixExp.CreateBuilder();
            node.Kind = SyntaxKind.ParenPrefixExp;
            node.StartPosition = this.textPosition;
            node.OpenParen = this.GetExpectedToken(SyntaxKind.OpenParen);
            node.Exp = this.ParseExpression().ToBuilder();
            node.CloseParen = this.GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.Kind = SyntaxKind.FunctionDef;
            node.StartPosition = this.textPosition;
            node.FunctionKeyword = this.GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FunctionBody = this.ParseFunctionBody().ToBuilder();
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
                node.PrefixExp = this.ParsePrefixExp().ToBuilder();
            }
            else
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }

            node.OpenBracket = this.GetExpectedToken(SyntaxKind.OpenBracket);
            node.Exp = this.ParseExpression().ToBuilder();
            node.CloseBracket = this.GetExpectedToken(SyntaxKind.CloseBracket);
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
                node.PrefixExp = this.ParsePrefixExp().ToBuilder();
            }
            else
            {
                node.PrefixExp = prefixExp.ToBuilder();
            }

            node.DotOperator = this.GetExpectedToken(SyntaxKind.Dot);
            node.NameIdentifier = this.GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private Var ParseVar()
        {
            switch (this.Peek().Kind)
            {
                case SyntaxKind.Identifier:
                    switch (this.Peek(2).Kind)
                    {
                        case SyntaxKind.OpenBracket:
                            return this.ParseSquareBracketVar(this.ParseNameVar());
                        case SyntaxKind.Dot:
                            return this.ParseDotVar(this.ParseNameVar());
                        case SyntaxKind.Colon:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.String:
                            var functionCallPrefixExp = this.ParseFunctionCallPrefixp(this.ParseNameVar());
                            return this.ParsePotentialVarWithPrefixExp(functionCallPrefixExp);
                        default:
                            return this.ParseNameVar();
                    }
                case SyntaxKind.OpenParen:
                    var parenPrefixExp = this.ParseParenPrefixExp();
                    return this.ParsePotentialVarWithPrefixExp(parenPrefixExp);
                default:
                    this.ParseErrorAtCurrentPosition(ErrorMessages.InvalidVar);

                    var missingToken = Token.CreateMissingToken(this.textPosition);
                    return NameVar.Create(SyntaxKind.MissingToken, this.textPosition, 0, missingToken);
            }
        }

        private NameVar ParseNameVar()
        {
            var node = NameVar.CreateBuilder();
            node.Kind = SyntaxKind.NameVar;
            node.StartPosition = this.textPosition;
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #endregion

        #region Args Nodes

        private Args ParseArgs()
        {
            switch (this.Peek().Kind)
            {
                case SyntaxKind.OpenParen:
                    return this.ParseParenArg();
                case SyntaxKind.OpenCurlyBrace:
                    return this.ParseTableConstructorArg();
                case SyntaxKind.String:
                    return this.ParseStringArg();
                default:
                    this.ParseErrorAtCurrentPosition(ErrorMessages.InvalidArgs);

                    var missingToken = Token.CreateMissingToken(this.textPosition);
                    return StringArg.Create(SyntaxKind.MissingToken, this.textPosition, 0, missingToken);
            }
        }

        private StringArg ParseStringArg()
        {
            var node = StringArg.CreateBuilder();
            node.Kind = SyntaxKind.StringArg;
            node.StartPosition = this.textPosition;
            node.StringLiteral = this.GetExpectedToken(SyntaxKind.String);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private TableContructorArg ParseTableConstructorArg()
        {
            var node = TableContructorArg.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorArg;
            node.StartPosition = this.textPosition;
            node.OpenCurly = this.GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = this.ParseFieldList().ToBuilder();
            node.CloseCurly = this.GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private ParenArg ParseParenArg()
        {
            var node = ParenArg.CreateBuilder();
            node.Kind = SyntaxKind.ParenArg;
            node.StartPosition = this.textPosition;
            node.OpenParen = this.GetExpectedToken(SyntaxKind.OpenParen);
            node.ExpList = this.ParseExpList().ToBuilder();
            node.CloseParen = this.GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region List Nodes

        private SeparatedList ParseSeparatedList(ParsingContext context)
        {
            this.contextStack.Push(context);
            var listNode = SeparatedList.CreateBuilder();
            var syntaxList = ImmutableList.CreateBuilder<SeparatedListElement>();
            listNode.Kind = this.GetListKind(context);
            listNode.StartPosition = this.textPosition;
            bool commaFound = false;

            while (true)
            {
                if (this.IsListElementBeginner(context, this.Peek().Kind))
                {
                    var node = SeparatedListElement.CreateBuilder();

                    node.StartPosition = this.textPosition;
                    node.Kind = SyntaxKind.SeparatedListElement;
                    node.Element = this.ParseListElement(context);

                    if (this.IsValidSeparator(context, this.Peek().Kind))
                    {
                        node.Seperator = this.NextToken();
                        commaFound = true;
                    }
                    else
                    {
                        commaFound = false;
                    }

                    node.Length = this.currentToken.End - node.StartPosition;

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

                if (this.IsListTerminator(context, this.Peek().Kind))
                {
                    break;
                }

                if (this.AbortParsingListOrMoveToNextToken(context))
                {
                    break;
                }
            }

            listNode.SyntaxList = syntaxList.ToImmutableList();

            listNode.Length = this.textPosition - listNode.StartPosition;

            if (this.contextStack.Count > 0 && this.contextStack.Peek() == context)
            {
                this.contextStack.Pop();
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
                    return this.ParseExpression();
                case ParsingContext.NameList:
                case ParsingContext.FuncNameDotSeperatedNameList:
                    if (this.ParseExpected(SyntaxKind.Identifier))
                    {
                        return this.currentToken;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                case ParsingContext.VarList:
                    return this.ParseVar();
                case ParsingContext.FieldList:
                    return this.ParseField();
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool IsListTerminator(ParsingContext context, SyntaxKind tokenType)
        {
            if (tokenType == SyntaxKind.EndOfFile || tokenType == SyntaxKind.EndKeyword)
            {
                return true;
            }

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
            return this.ParseSeparatedList(ParsingContext.VarList);
        }

        private ParList ParseParList()
        {
            if (this.Peek().Kind == SyntaxKind.VarArgOperator)
            {
                var node = VarArgParList.CreateBuilder();
                node.Kind = SyntaxKind.VarArgPar;
                node.StartPosition = this.textPosition;
                node.VarargOperator = this.GetExpectedToken(SyntaxKind.VarArgOperator);
                node.Length = this.textPosition - node.StartPosition;
                return node.ToImmutable();
            }
            else
            {
                var node = NameListPar.CreateBuilder();
                node.Kind = SyntaxKind.NameListPar;
                node.StartPosition = this.textPosition;
                node.NamesList = this.ParseNameList().ToBuilder();
                if (this.ParseExpected(SyntaxKind.Comma))
                {
                    node.Comma = this.currentToken;
                    node.Vararg = this.GetExpectedToken(SyntaxKind.VarArgOperator);
                }
                node.Length = this.textPosition - node.StartPosition;
                return node.ToImmutable();
            }
        }

        private SeparatedList ParseNameList()
        {
            return this.ParseSeparatedList(ParsingContext.NameList);
        }

        private SeparatedList ParseExpList()
        {
            return this.ParseSeparatedList(ParsingContext.ExpList);
        }

        #endregion

        #endregion

        private FuncBodyNode ParseFunctionBody()
        {
            var node = FuncBodyNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncBodyNode;
            node.StartPosition = this.textPosition;
            node.OpenParen = this.GetExpectedToken(SyntaxKind.OpenParen);
            node.ParameterList = this.ParseParList().ToBuilder();
            node.CloseParen = this.GetExpectedToken(SyntaxKind.CloseParen);
            node.Block = this.ParseBlock(ParsingContext.FuncBodyBlock).ToBuilder();
            node.EndKeyword = this.GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = this.textPosition - node.StartPosition;
            return node.ToImmutable();
        }

        private FuncNameNode ParseFuncNameNode()
        {
            var node = FuncNameNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncNameNode;
            node.StartPosition = this.textPosition;
            node.Name = this.GetExpectedToken(SyntaxKind.Identifier);

            node.FuncNameList = this.ParseSeparatedList(ParsingContext.FuncNameDotSeperatedNameList).ToBuilder();
            if (this.ParseExpected(SyntaxKind.Colon))
            {
                node.OptionalColon = this.currentToken;
                node.OptionalName = this.GetExpectedToken(SyntaxKind.Identifier);
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
            if (this.positionInTokenList == -1)
            {
                this.errorList.Add(new ParseError(message, 0, 0));
            }
            else
            {
                this.errorList.Add(new ParseError(message, this.currentToken.Start, this.currentToken.End));
            }
        }

        private void ParseErrorAtCurrentPosition(string message)
        {
            this.errorList.Add(new ParseError(message, this.Peek().FullStart, this.Peek().FullStart));
        }

        private bool AbortParsingListOrMoveToNextToken(ParsingContext context)
        {
            this.ParseErrorAtCurrentToken(this.GetContextError(context));
            if (this.isInSomeParsingContext())
            {
                return true;
            }

            this.SkipCurrentToken();
            return false;
        }

        private void SkipCurrentToken(string message = null)
        {
            this.NextToken();

            var tempTriviaList = this.currentToken.LeadingTrivia;
            tempTriviaList.Add(new Trivia(this.currentToken.Kind, this.currentToken.Text));

            foreach (var triv in this.Peek().LeadingTrivia)
            {
                tempTriviaList.Add(triv);
            }

            var tokenWithAddedTrivia = new Token(this.Peek().Kind, this.Peek().Text, tempTriviaList, this.currentToken.FullStart, this.textPosition);

            this.tokenList.RemoveAt(this.positionInTokenList);

            if (this.positionInTokenList > this.tokenList.Count)
            {
                this.tokenList.Add(tokenWithAddedTrivia);
            }
            else
            {
                this.tokenList[this.positionInTokenList] = tokenWithAddedTrivia;
            }

            this.SetPositionInTokenList(this.positionInTokenList - 1);
            this.currentToken = (this.positionInTokenList >= 0) ? this.currentToken = this.tokenList[this.positionInTokenList] : null;
        }

        private bool isInSomeParsingContext()
        {
            var tempStack = new Stack<ParsingContext>(this.contextStack.Reverse());

            while (this.contextStack.Count > 0)
            {
                if (this.IsListElementBeginner(this.contextStack.Peek(), this.Peek().Kind))
                {
                    return true;
                }
                else
                {
                    this.contextStack.Pop();
                }
            }

            this.contextStack = tempStack;
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
            if (this.Peek().Kind == SyntaxKind.OpenBracket)
            {
                return this.ParseSquareBracketVar(prefixExp);
            }
            else
            {
                //Arbitrarily assuming it's a dot var as that is the only valid alternative
                //when there is no open bracket... will log errors, if the requirements for dot var are not found
                return this.ParseDotVar(prefixExp);
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