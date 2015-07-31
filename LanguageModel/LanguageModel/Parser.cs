using LanguageService.LanguageModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace LanguageService
{
    public class Parser
    {
        private Stack<ParsingContext> contextStack;
        private Token currentToken;
        private List<Token> tokenList;
        private int positionInTokenList;
        private List<ParseError> errorList;

        public Parser()
        {
            contextStack = new Stack<ParsingContext>();
            errorList = new List<ParseError>();
            positionInTokenList = -1;
        }

        public SyntaxTree CreateSyntaxTree(Stream luaStream)
        {
            positionInTokenList = -1;  //Make sure that internal state is at "beginning"
            tokenList = Lexer.Tokenize(luaStream);
            ChunkNode root = ParseChunkNode();
            return new SyntaxTree(root, errorList.ToImmutableList());
        }

        #region tokenList Accessors 
        private Token NextToken()
        {
            if (positionInTokenList + 1 < tokenList.Count)
            {
                currentToken = tokenList[++positionInTokenList];
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
                ParseErrorAtCurrentPosition(ErrorMessages.MissingToken + kind.ToString());
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
                return tokenList[tokenList.Count - 1];
            }
        }

        private Token Peek(int lookaheadAmount)
        {
            if (positionInTokenList + lookaheadAmount < tokenList.Count)
            {
                return tokenList[positionInTokenList + lookaheadAmount];
            }
            else
            {
                return tokenList[tokenList.Count - lookaheadAmount];
            }
        }
        #endregion

        #region Parse Methods
        private ChunkNode ParseChunkNode()
        {
            var node = ChunkNode.CreateBuilder();
            node.Kind = SyntaxKind.ChunkNode;
            node.StartPosition = Peek().FullStart;
            node.ProgramBlock = ParseBlock(ParsingContext.ChunkNodeBlock)?.ToBuilder();
            node.EndOfFile = GetExpectedToken(SyntaxKind.EndOfFile);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private BlockNode ParseBlock(ParsingContext context)
        {
            contextStack.Push(context);
            var node = BlockNode.CreateBuilder();
            node.Kind = node.Kind = SyntaxKind.BlockNode;
            node.StartPosition = this.Peek().FullStart;
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
                    AbortParsingListOrMoveToNextToken(context);
                }
            }

            node.Statements = children.ToImmutableList();

            node.Length = currentToken.End - node.StartPosition;
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
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Simple Statement Nodes

        private AssignmentStatementNode ParseAssignmentStatementNode()
        {
            var node = AssignmentStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.AssignmentStatementNode;
            node.VarList = ParseVarList()?.ToBuilder();
            node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.ExpList = ParseExpList()?.ToBuilder();
            return node.ToImmutable();
        }

        private MultipleArgForStatementNode ParseMultipleArgForStatementNode()
        {
            var node = MultipleArgForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.MultipleArgForStatementNode;
            node.StartPosition = Peek().Start;
            node.ForKeyword = GetExpectedToken(SyntaxKind.ForKeyword);
            node.NameList = ParseNamesList()?.ToBuilder();
            node.InKeyword = GetExpectedToken(SyntaxKind.InKeyword);
            node.ExpList = ParseExpList()?.ToBuilder();
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.ForStatementBlock)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private SimpleForStatementNode ParseSimpleForStatementNode()
        {
            var node = SimpleForStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.SimpleForStatementNode;
            node.StartPosition = Peek().Start;
            node.ForKeyword = GetExpectedToken(SyntaxKind.ForKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
            node.Exp1 = ParseExpression()?.ToBuilder();
            node.Comma = GetExpectedToken(SyntaxKind.Comma);
            node.Exp2 = ParseExpression()?.ToBuilder();

            if (Peek().Kind == SyntaxKind.Comma)
            {
                node.OptionalComma = GetExpectedToken(SyntaxKind.Comma);
                node.OptionalExp3 = ParseExpression()?.ToBuilder();
            }

            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.ForStatementBlock)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalAssignmentStatementNode ParseLocalAssignmentStatementNode()
        {
            var node = LocalAssignmentStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalAssignmentStatementNode;
            node.StartPosition = Peek().Start;
            node.LocalKeyword = GetExpectedToken(SyntaxKind.LocalKeyword);
            node.NameList = ParseNamesList()?.ToBuilder();

            if (Peek().Kind == SyntaxKind.AssignmentOperator)
            {
                node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
                node.ExpList = ParseExpList()?.ToBuilder();
            }

            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalFunctionStatementNode ParseLocalFunctionStatementNode()
        {
            var node = LocalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LocalFunctionStatementNode;
            node.StartPosition = Peek().Start;
            node.LocalKeyword = GetExpectedToken(SyntaxKind.LocalKeyword);
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.FuncBody = ParseFunctionBody()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LabelStatementNode ParseLabelStatementNode()
        {
            var node = LabelStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.LabelStatementNode;
            node.StartPosition = Peek().Start;
            node.DoubleColon1 = GetExpectedToken(SyntaxKind.DoubleColon);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.DoubleColon2 = GetExpectedToken(SyntaxKind.DoubleColon);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private GlobalFunctionStatementNode ParseGlobalFunctionStatementNode()
        {
            var node = GlobalFunctionStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GlobalFunctionStatementNode;
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FuncName = ParseFuncNameNode()?.ToBuilder();
            node.FuncBody = ParseFunctionBody()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private RepeatStatementNode ParseRepeatStatementNode()
        {
            var node = RepeatStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.RepeatStatementNode;
            node.StartPosition = Peek().Start;
            node.RepeatKeyword = GetExpectedToken(SyntaxKind.RepeatKeyword);
            node.Block = ParseBlock(ParsingContext.RepeatStatementBlock)?.ToBuilder();
            node.UntilKeyword = GetExpectedToken(SyntaxKind.UntilKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private WhileStatementNode ParseWhileStatementNode()
        {
            var node = WhileStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.WhileStatementNode;
            node.StartPosition = Peek().Start;
            node.WhileKeyword = GetExpectedToken(SyntaxKind.WhileKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.WhileBlock)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private DoStatementNode ParseDoStatementNode()
        {
            var node = DoStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.DoStatementNode;
            node.StartPosition = Peek().Start;
            node.DoKeyword = GetExpectedToken(SyntaxKind.DoKeyword);
            node.Block = ParseBlock(ParsingContext.DoStatementBlock)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private BreakStatementNode ParseBreakStatementNode()
        {
            var node = BreakStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.BreakStatementNode;
            node.StartPosition = Peek().Start;
            node.BreakKeyword = GetExpectedToken(SyntaxKind.BreakKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private GoToStatementNode ParseGoToStatementNode()
        {
            var node = GoToStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.GoToStatementNode;
            node.StartPosition = Peek().Start;
            node.GoToKeyword = GetExpectedToken(SyntaxKind.GotoKeyword);
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private IfStatementNode ParseIfStatementNode()
        {
            var node = IfStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.IfStatementNode;
            node.StartPosition = Peek().Start;
            node.IfKeyword = GetExpectedToken(SyntaxKind.IfKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.ThenKeyword = GetExpectedToken(SyntaxKind.ThenKeyword);
            node.IfBlock = ParseBlock(ParsingContext.IfBlock)?.ToBuilder();

            if (Peek().Kind == SyntaxKind.ElseIfKeyword)
            {
                node.ElseIfList = ParseElseIfList();
            }

            if (Peek().Kind == SyntaxKind.ElseKeyword)
            {
                node.ElseBlock = ParseElseBlock()?.ToBuilder();
            }

            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private ReturnStatementNode ParseRetStat()
        {
            var node = ReturnStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.ReturnStatementNode;
            node.StartPosition = currentToken.Start;
            node.ReturnKeyword = currentToken;
            node.ReturnExpressions = ParseExpList()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FunctionCallStatementNode ParseFunctionCallStatementNode()
        {
            var node = FunctionCallStatementNode.CreateBuilder();
            node.Kind = SyntaxKind.FunctionCallStatementNode;
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParseNameVar()?.ToBuilder();

            if (ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = ParseArgs()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region If Statement Nodes

        private ElseBlockNode ParseElseBlock()
        {
            var node = ElseBlockNode.CreateBuilder();
            node.Kind = SyntaxKind.ElseBlockNode;
            node.StartPosition = currentToken.Start;
            node.ElseKeyword = currentToken;
            node.Block = ParseBlock(ParsingContext.ElseBlock)?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ImmutableList<ElseIfBlockNode> ParseElseIfList()
        {
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
            node.StartPosition = currentToken.Start;
            node.ElseIfKeyword = currentToken;
            node.Exp = ParseExpression()?.ToBuilder();
            node.ThenKeyword = GetExpectedToken(SyntaxKind.ThenKeyword);
            node.Block = ParseBlock(ParsingContext.ElseIfBlock)?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region Expression Nodes

        private ExpressionNode ParseExpression()
        {
            ExpressionNode exp;
            int start = Peek().Start;
            if (ParseExpected(SyntaxKind.LengthUnop)
                || ParseExpected(SyntaxKind.NotUnop)
                || ParseExpected(SyntaxKind.MinusOperator)
                || ParseExpected(SyntaxKind.TildeUnOp))
            {
                var node = UnaryOperatorExpression.CreateBuilder();
                node.StartPosition = currentToken.Start;
                node.UnaryOperator = currentToken;
                node.Exp = ParseExpression()?.ToBuilder();
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
                        exp = SimpleExpression.Create(SyntaxKind.SimpleExpression, currentToken.Start, currentToken.Length, currentToken);
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
                        exp = ParsePrefixExp();
                        break;
                    default:
                        //TODO: @Cyrusskip token?
                        throw new InvalidOperationException();
                }
            }

            if (IsBinop(Peek().Kind))
            {
                var binop = NextToken();
                return BinaryOperatorExpression.Create(SyntaxKind.BinaryOperatorExpression, start, binop.End - start, exp, binop, ParseExpression()); //Question... this representation is slightly flawed if "operator precedence" matters... but does it matter? What scenario from a tooling perspective cares about precedence?
            }
            else
            {
                return exp;
            }
        }

        private FunctionCallExp ParseFunctionCallExp()
        {
            var node = FunctionCallExp.CreateBuilder();
            node.Kind = SyntaxKind.FunctionCallExp;
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParseNameVar()?.ToBuilder();

            if (ParseExpected(SyntaxKind.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(SyntaxKind.Identifier);
            }

            node.Args = ParseArgs()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructorExp ParseTableConstructorExp()
        {
            var node = TableConstructorExp.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorExp;
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        #region Field Nodes

        private FieldList ParseFieldList()
        {
            var node = FieldList.CreateBuilder();
            node.Kind = SyntaxKind.FieldList;
            node.StartPosition = Peek().Start;
            bool parseFields = true;
            var fields = new List<FieldAndSeperatorPair>();

            while (parseFields)
            {
                var fieldAndSep = FieldAndSeperatorPair.CreateBuilder();
                fieldAndSep.Field = ParseField()?.ToBuilder();
                if (ParseExpected(SyntaxKind.Comma) || ParseExpected(SyntaxKind.Semicolon))
                {
                    fieldAndSep.FieldSeparator = currentToken;
                    parseFields = true;
                }
                else
                {
                    if (Peek().Kind == SyntaxKind.CloseCurlyBrace)
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
                        node.StartPosition = Peek().Start;
                        node.Name = GetExpectedToken(SyntaxKind.Identifier);
                        node.AssignmentOperator = GetExpectedToken(SyntaxKind.AssignmentOperator);
                        node.Exp = ParseExpression()?.ToBuilder();
                        node.Length = currentToken.End - node.StartPosition;
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
            node.StartPosition = currentToken.Start;
            node.OpenBracket = GetExpectedToken(SyntaxKind.OpenBracket);
            node.IdentifierExp = ParseExpression()?.ToBuilder();
            node.CloseBracket = GetExpectedToken(SyntaxKind.CloseBracket);
            node.AssignedExp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ExpField ParseExpField()
        {
            var node = ExpField.CreateBuilder();
            node.Kind = SyntaxKind.ExpField;
            node.StartPosition = currentToken.End;
            node.Exp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region PrefixExp Expression

        private PrefixExp ParsePrefixExp()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.OpenParen:
                    return ParseParenPrefixExp();
                case SyntaxKind.Identifier:
                    switch (Peek(2).Kind)
                    {
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.String:
                        case SyntaxKind.Colon:
                            return ParseFunctionCallExp();
                        case SyntaxKind.Dot:
                            return ParseDotVar();
                        case SyntaxKind.OpenBracket:
                            return ParseSquareBracketVar();
                        default:
                            return ParseNameVar();
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        private ParenPrefixExp ParseParenPrefixExp()
        {
            var node = ParenPrefixExp.CreateBuilder();
            node.Kind = SyntaxKind.ParenPrefixExp;
            node.StartPosition = currentToken.Start;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.Exp = ParseExpression()?.ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = currentToken.End;
            return node.ToImmutable();
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.Kind = SyntaxKind.FunctionDef;
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = GetExpectedToken(SyntaxKind.FunctionKeyword);
            node.FunctionBody = ParseFunctionBody()?.ToBuilder();
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private SquareBracketVar ParseSquareBracketVar()
        {
            var node = SquareBracketVar.CreateBuilder();
            node.Kind = SyntaxKind.SquareBracketVar;
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParsePrefixExp()?.ToBuilder();
            node.OpenBracket = GetExpectedToken(SyntaxKind.OpenBracket);
            node.Exp = ParseExpression()?.ToBuilder();
            node.CloseBracket = GetExpectedToken(SyntaxKind.CloseBracket);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private DotVar ParseDotVar()
        {
            var node = DotVar.CreateBuilder();
            node.Kind = SyntaxKind.DotVar;
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParseNameVar()?.ToBuilder();
            node.DotOperator = GetExpectedToken(SyntaxKind.Dot);
            node.NameIdentifier = GetExpectedToken(SyntaxKind.Identifier);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private Var ParseVar()
        {
            switch (Peek().Kind)
            {
                case SyntaxKind.OpenParen:
                    return ParsePotentialVarWithPrefixExp();
                case SyntaxKind.Identifier:
                    switch (Peek(2).Kind)
                    {
                        case SyntaxKind.OpenBracket:
                            return ParseSquareBracketVar();
                        case SyntaxKind.Dot:
                            return ParseDotVar();
                        case SyntaxKind.Colon:
                        case SyntaxKind.OpenCurlyBrace:
                        case SyntaxKind.OpenParen:
                        case SyntaxKind.String:
                            return ParsePotentialVarWithPrefixExp();
                        default:
                            return ParseNameVar();
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        private NameVar ParseNameVar()
        {
            var node = NameVar.CreateBuilder();
            node.Kind = SyntaxKind.NameVar;
            node.StartPosition = currentToken.Start;
            node.Name = currentToken;
            node.Length = currentToken.End - node.StartPosition;
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
                    throw new InvalidOperationException();
            }
        }

        private StringArg ParseStringArg()
        {
            var node = StringArg.CreateBuilder();
            node.Kind = SyntaxKind.StringArg;
            node.StartPosition = currentToken.Start;
            node.StringLiteral = GetExpectedToken(SyntaxKind.String);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableContructorArg ParseTableConstructorArg()
        {
            var node = TableContructorArg.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorArg;
            node.StartPosition = currentToken.Start;
            node.OpenCurly = GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ParenArg ParseParenArg()
        {
            var node = ParenArg.CreateBuilder();
            node.Kind = SyntaxKind.ParenArg;
            node.StartPosition = Peek().Start;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.ExpList = ParseExpList()?.ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        #endregion

        #region List Nodes

        private SeparatedList ParseSeperatedList(ParsingContext context)
        {
            //TODO: return pre-stored empty list if empty...
            contextStack.Push(context);
            var listNode = SeparatedList.CreateBuilder();
            listNode.Kind = GetListKind(context);
            listNode.StartPosition = Peek().Start;
            bool commaFound = false;

            while (true)
            {
                if (IsListElementBeginner(context, Peek().Kind))
                {
                    var node = SeparatedListElement.CreateBuilder();
                    node.StartPosition = Peek().Start;
                    node.Kind = SyntaxKind.SeparatedListElement;
                    node.Element = ParseListElement(context);

                    if (IsValidSeparator(context, Peek().Kind))
                    {
                        node.Seperator = NextToken();
                        commaFound = true;
                    }

                    node.Length = currentToken.End - node.StartPosition;

                    listNode.SyntaxList.Add(node.ToImmutable());

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

            listNode.Length = currentToken.End - listNode.StartPosition;
            contextStack.Pop();

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

        //TODO: @ Cyrus... syntaxnodeortoken seems lose?
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
                case ParsingContext.NameList:
                case ParsingContext.FuncNameDotSeperatedNameList:
                    return false; //TODO: Confirm there is no concretely defined terminator...
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
                case ParsingContext.FieldList:
                    if (tokenType == SyntaxKind.Identifier || tokenType == SyntaxKind.OpenBracket)
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

        private bool AbortParsingListOrMoveToNextToken(ParsingContext context)
        {
            ParseErrorAtCurrentToken(GetContextError(context));
            if (isInSomeParsingContext())
            {
                return true;
            }

            NextToken();
            ParseErrorAtCurrentToken(ErrorMessages.SkippedToken + '"' + currentToken.Text + '"');
            return false;
        }

        private bool isInSomeParsingContext()
        {
            Stack<ParsingContext> tempStack = contextStack;

            while (contextStack.Count > 0)
            {
                if (IsListElementBeginner(contextStack.Pop(), Peek().Kind))
                {
                    return true;
                }
            }

            contextStack = tempStack;
            return false;
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
        private VarList ParseVarList()
        {
            contextStack.Push(ParsingContext.VarList);
            var node = VarList.CreateBuilder();
            node.Kind = SyntaxKind.VarList;
            node.StartPosition = Peek().Start;

            var vars = new List<CommaVarPair>();

            vars.Add(CommaVarPair.Create(null, ParseVar()));

            while (ParseExpected(SyntaxKind.Comma))
            {
                vars.Add(CommaVarPair.Create(currentToken, ParseVar()));
            }

            node.Vars = vars.ToImmutableList();

            node.Length = currentToken.End - node.StartPosition;
            contextStack.Pop();
            return node.ToImmutable();
        }

        private ParList ParseParList()
        {
            if (Peek().Kind == SyntaxKind.VarArgOperator)
            {
                var node = VarArgPar.CreateBuilder();
                node.Kind = SyntaxKind.VarArgPar;
                node.StartPosition = Peek().Start;
                node.VarargOperator = GetExpectedToken(SyntaxKind.VarArgOperator);
                node.Length = currentToken.End - node.StartPosition;
                return node.ToImmutable();
            }
            else
            {
                var node = NameListPar.CreateBuilder();
                node.Kind = SyntaxKind.NameListPar;
                node.StartPosition = Peek().Start;
                node.NamesList = ParseNamesList()?.ToBuilder();
                if (ParseExpected(SyntaxKind.Comma))
                {
                    node.Comma = currentToken;
                    node.Vararg = GetExpectedToken(SyntaxKind.VarArgOperator);
                }
                node.Length = currentToken.End - node.StartPosition;
                return node.ToImmutable();
            }
        }

        private NameList ParseNamesList()
        {
            contextStack.Push(ParsingContext.NameList);

            var node = NameList.CreateBuilder();
            node.Kind = SyntaxKind.NameList;
            node.StartPosition = Peek().Start;

            List<NameCommaPair> names = new List<NameCommaPair>();

            //Add initial mandatory name with no preceding comma
            names.Add(NameCommaPair.Create(null, currentToken));

            while (ParseExpected(SyntaxKind.Comma))
            {
                names.Add(NameCommaPair.Create(currentToken, GetExpectedToken(SyntaxKind.Identifier)));
            }

            node.Names = names.ToImmutableList();
            node.Length = currentToken.End - node.StartPosition;

            contextStack.Pop();
            return node.ToImmutable();
        }

        private ExpList ParseExpList()
        {
            contextStack.Push(ParsingContext.ExpList);

            var node = ExpList.CreateBuilder();
            node.Kind = SyntaxKind.ExpField;
            node.StartPosition = Peek().Start;

            List<ExpressionCommaPair> expressions = new List<ExpressionCommaPair>();

            ExpressionNode exp = ParseExpression();
            if (exp != null)
            {
                expressions.Add(ExpressionCommaPair.Create(null, exp));

                while (ParseExpected(SyntaxKind.Comma))
                {
                    expressions.Add(ExpressionCommaPair.Create(currentToken, ParseExpression()));
                }
            }

            node.Expressions = expressions.ToImmutableList();
            node.Length = currentToken.End - node.StartPosition;

            contextStack.Pop();
            return node.ToImmutable();
        }

        #endregion

        #endregion

        private FuncBodyNode ParseFunctionBody()
        {
            var node = FuncBodyNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncBodyNode;
            node.StartPosition = Peek().Start;
            node.OpenParen = GetExpectedToken(SyntaxKind.OpenParen);
            node.ParameterList = ParseParList()?.ToBuilder();
            node.CloseParen = GetExpectedToken(SyntaxKind.CloseParen);
            node.Block = ParseBlock(ParsingContext.FuncBodyBlock)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(SyntaxKind.EndKeyword);
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private TableConstructorNode ParseTableConstructor()
        {
            var node = TableConstructorNode.CreateBuilder();
            node.Kind = SyntaxKind.TableConstructorNode;
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(SyntaxKind.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(SyntaxKind.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FuncNameNode ParseFuncNameNode()
        {
            var node = FuncNameNode.CreateBuilder();
            node.Kind = SyntaxKind.FuncNameNode;
            node.StartPosition = Peek().Start;
            node.Name = GetExpectedToken(SyntaxKind.Identifier);
            var names = new List<NameDotPair>();

            while (ParseExpected(SyntaxKind.Dot))
            {
                if (Peek().Kind == SyntaxKind.Identifier)
                {
                    names.Add(NameDotPair.Create(currentToken, NextToken()));
                }
                else
                {
                    names.Add(NameDotPair.Create(currentToken, null));
                }
            }

            node.Length = currentToken.End - node.StartPosition;
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
            //TODO: test to make sure method is only called after the "error token" is consumed
            errorList.Add(new ParseError(message, currentToken.Start, currentToken.End));
        }

        private void ParseErrorAtCurrentPosition(string message)
        {
            //TODO: test is Peek().FullStart accurate?
            errorList.Add(new ParseError(message, Peek().FullStart, Peek().FullStart));
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

        private Var ParsePotentialVarWithPrefixExp()
        {
            int tempPosition = positionInTokenList;
            ParsePrefixExp(); //Skip to the end of the prefix exp before checking.
            if (Peek().Kind == SyntaxKind.OpenBracket)
            {
                positionInTokenList = tempPosition; //Restore tokenList to beginning of node
                return ParseSquareBracketVar();
            }
            else
            {
                //This case has arbitrarily chosen DotVar as the default for incomplete Vars starting with prefixexps
                positionInTokenList = tempPosition; //Restore tokenList to beginning of node
                return ParseDotVar();
            }
        }

        #endregion
    }
}