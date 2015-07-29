using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace LanguageService
{
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

        private bool ParseExpected(TokenType type)
        {
            int tempIndex = positionInTokenList + 1;
            if (tempIndex < tokenList.Count)
            {
                if (Peek().Type == type)
                {
                    currentToken = NextToken();
                    return true;
                }
            }
            return false;
        }

        private Token GetExpectedToken(TokenType type)
        {
            if (ParseExpected(type))
            {
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
            node.StartPosition = Peek().FullStart;
            node.ProgramBlock = ParseBlock(Context.ProgramContext)?.ToBuilder();
            node.EndOfFile = GetExpectedToken(TokenType.EndOfFile);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private BlockNode ParseBlock(Context parsingContext)
        {
            contextStack.Push(parsingContext);
            var node = BlockNode.CreateBuilder();
            node.StartPosition = this.Peek().FullStart;
            List<StatementNode> children = new List<StatementNode>();

            while (!IsContextTerminator(parsingContext, Peek().Type))
            {
                children.Add(ParseStatement());
            }

            node.Statements = children.ToImmutableList();

            if (Peek().Type == TokenType.ReturnKeyword)
            {
                node.ReturnStatement = ParseRetStat()?.ToBuilder();
            }

            node.Length = currentToken.End - node.StartPosition;
            contextStack.Pop();
            return node.ToImmutable();
        }

        private StatementNode ParseStatement()
        {
            switch (Peek().Type)
            {
                case TokenType.Semicolon:
                    NextToken();
                    return SemiColonStatementNode.Create(currentToken.Start, currentToken.Length, currentToken); //Question: could just use next token as first param
                case TokenType.BreakKeyword:
                    return ParseBreakStatementNode();
                case TokenType.GotoKeyword:
                    return ParseGoToStatementNode();
                case TokenType.DoKeyword:
                    return ParseDoStatementNode();
                case TokenType.WhileKeyword:
                    return ParseWhileStatementNode();
                case TokenType.RepeatKeyword:
                    return ParseRepeatStatementNode();
                case TokenType.IfKeyword:
                    return ParseIfStatementNode();
                case TokenType.ForKeyword:
                    if (Peek(2).Type == TokenType.Identifier && tokenList[positionInTokenList + 3].Type == TokenType.AssignmentOperator)
                    {
                        return ParseSimpleForStatementNode();
                    }
                    else
                    {
                        return ParseMultipleArgForStatementNode();
                    }
                case TokenType.FunctionKeyword:
                    return ParseGlobalFunctionStatementNode();
                case TokenType.DoubleColon:
                    return ParseLabelStatementNode();
                case TokenType.LocalKeyword:
                    if (Peek(2).Type == TokenType.FunctionKeyword)
                    {
                        return ParseLocalFunctionStatementNode();
                    }
                    else
                    {
                        return ParseLocalAssignmentStatementNode();
                    }
                case TokenType.Identifier:
                    switch (Peek(2).Type)
                    {
                        case TokenType.OpenCurlyBrace:
                        case TokenType.OpenParen:
                        case TokenType.String:
                        case TokenType.Colon:
                            return ParseFunctionCallStatementNode();
                        default:
                            return ParseAssignmentStatementNode();
                    }
                default:
                    //TODO turn into skipped Trivia;
                    NextToken();
                    return null;
            }
        }

        private AssignmentStatementNode ParseAssignmentStatementNode()
        {
            var node = AssignmentStatementNode.CreateBuilder();
            node.VarList = ParseVarList()?.ToBuilder();
            node.AssignmentOperator = GetExpectedToken(TokenType.AssignmentOperator);
            node.ExpList = ParseExpList()?.ToBuilder();
            return node.ToImmutable();
        }

        private VarList ParseVarList()
        {
            contextStack.Push(Context.VarListContext);
            var node = VarList.CreateBuilder();
            node.StartPosition = Peek().Start;

            var vars = new List<CommaVarPair>();

            vars.Add(CommaVarPair.Create(null, ParseVar()));

            while(ParseExpected(TokenType.Comma))
            {
                vars.Add(CommaVarPair.Create(currentToken, ParseVar()));
            }

            node.Vars = vars.ToImmutableList();

            node.Length = currentToken.End - node.StartPosition;
            contextStack.Pop();
            return node.ToImmutable();
        }

        private Var ParseVar()
        {
            switch(Peek().Type)
            {
                case TokenType.OpenParen:
                    return ParsePotentialVarWithPrefixExp();
                case TokenType.Identifier:
                    switch(Peek(2).Type)
                    {
                        case TokenType.OpenBracket:
                            return ParseSquareBracketVar();
                        case TokenType.Dot:
                            return ParseDotVar();
                        case TokenType.Colon:
                        case TokenType.OpenCurlyBrace:
                        case TokenType.OpenParen:
                        case TokenType.String:
                            return ParsePotentialVarWithPrefixExp();
                        default:
                            return ParseNameVar();
                    }
                default:
                    return null;
            }
        }

        private MultipleArgForStatementNode ParseMultipleArgForStatementNode()
        {
            var node = MultipleArgForStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.ForKeyword = GetExpectedToken(TokenType.ForKeyword);
            node.NameList = ParseNamesList()?.ToBuilder();
            node.InKeyword = GetExpectedToken(TokenType.InKeyword);
            node.ExpList = ParseExpList()?.ToBuilder();
            node.DoKeyword = GetExpectedToken(TokenType.DoKeyword);
            node.Block = ParseBlock(Context.ForStatementContext)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private SimpleForStatementNode ParseSimpleForStatementNode()
        {
            var node = SimpleForStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.ForKeyword = GetExpectedToken(TokenType.ForKeyword);
            node.Name = GetExpectedToken(TokenType.Identifier);
            node.AssignmentOperator = GetExpectedToken(TokenType.AssignmentOperator);
            node.Exp1 = ParseExpression()?.ToBuilder();
            node.Comma = GetExpectedToken(TokenType.Comma);
            node.Exp2 = ParseExpression()?.ToBuilder();

            if (Peek().Type == TokenType.Comma)
            {
                node.OptionalComma = GetExpectedToken(TokenType.Comma);
                node.OptionalExp3 = ParseExpression()?.ToBuilder();
            }

            node.DoKeyword = GetExpectedToken(TokenType.DoKeyword);
            node.Block = ParseBlock(Context.ForStatementContext)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalAssignmentStatementNode ParseLocalAssignmentStatementNode()
        {
            var node = LocalAssignmentStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.LocalKeyword = GetExpectedToken(TokenType.LocalKeyword);
            node.NameList = ParseNamesList()?.ToBuilder();

            if (Peek().Type == TokenType.AssignmentOperator)
            {
                node.AssignmentOperator = GetExpectedToken(TokenType.AssignmentOperator);
                node.ExpList = ParseExpList()?.ToBuilder();
            }

            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LocalFunctionStatementNode ParseLocalFunctionStatementNode()
        {
            var node = LocalFunctionStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.LocalKeyword = GetExpectedToken(TokenType.LocalKeyword);
            node.FunctionKeyword = GetExpectedToken(TokenType.FunctionKeyword);
            node.Name = GetExpectedToken(TokenType.Identifier);
            node.FuncBody = ParseFunctionBody()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private LabelStatementNode ParseLabelStatementNode()
        {
            var node = LabelStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.DoubleColon1 = GetExpectedToken(TokenType.DoubleColon);
            node.Name = GetExpectedToken(TokenType.Identifier);
            node.DoubleColon2 = GetExpectedToken(TokenType.DoubleColon);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private GlobalFunctionStatementNode ParseGlobalFunctionStatementNode()
        {
            var node = GlobalFunctionStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = GetExpectedToken(TokenType.FunctionKeyword);
            node.FuncName = ParseFuncNameNode()?.ToBuilder();
            node.FuncBody = ParseFunctionBody()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FuncNameNode ParseFuncNameNode()
        {
            var node = FuncNameNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.Name = GetExpectedToken(TokenType.Identifier);
            var names = new List<NameDotPair>();

            while (ParseExpected(TokenType.Dot))
            {
                if (Peek().Type == TokenType.Identifier)
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

        private RepeatStatementNode ParseRepeatStatementNode()
        {
            var node = RepeatStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.RepeatKeyword = GetExpectedToken(TokenType.RepeatKeyword);
            node.Block = ParseBlock(Context.RepeatStatementContext)?.ToBuilder();
            node.UntilKeyword = GetExpectedToken(TokenType.UntilKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private WhileStatementNode ParseWhileStatementNode()
        {
            var node = WhileStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.WhileKeyword = GetExpectedToken(TokenType.WhileKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.DoKeyword = GetExpectedToken(TokenType.DoKeyword);
            node.Block = ParseBlock(Context.WhileContext)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private DoStatementNode ParseDoStatementNode()
        {
            var node = DoStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.DoKeyword = GetExpectedToken(TokenType.DoKeyword);
            node.Block = ParseBlock(Context.DoStatementContext)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private BreakStatementNode ParseBreakStatementNode()
        {
            var node = BreakStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.BreakKeyword = GetExpectedToken(TokenType.BreakKeyword);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private GoToStatementNode ParseGoToStatementNode()
        {
            var node = GoToStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.GoToKeyword = GetExpectedToken(TokenType.GotoKeyword);
            node.Name = GetExpectedToken(TokenType.Identifier);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private IfStatementNode ParseIfStatementNode()
        {
            var node = IfStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.IfKeyword = GetExpectedToken(TokenType.IfKeyword);
            node.Exp = ParseExpression()?.ToBuilder();
            node.ThenKeyword = GetExpectedToken(TokenType.ThenKeyword);
            node.IfBlock = ParseBlock(Context.IfBlockContext)?.ToBuilder();

            if (Peek().Type == TokenType.ElseIfKeyword)
            {
                node.ElseIfList = ParseElseIfList();
            }

            if (Peek().Type == TokenType.ElseKeyword)
            {
                node.ElseBlock = ParseElseBlock()?.ToBuilder();
            }

            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private ExpressionNode ParseExpression()
        {
            ExpressionNode exp;
            int start = Peek().Start;
            if (ParseExpected(TokenType.LengthUnop)
                || ParseExpected(TokenType.NotUnop)
                || ParseExpected(TokenType.MinusOperator)
                || ParseExpected(TokenType.TildeUnOp))
            {
                var node = UnaryOperatorExpression.CreateBuilder();
                node.StartPosition = currentToken.Start;
                node.UnaryOperator = currentToken;
                node.Exp = ParseExpression()?.ToBuilder();
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
                        exp = null; //Question @cyrus: correct?
                        break;
                }
            }

            if (IsBinop(Peek().Type))
            {
                var binop = NextToken();
                return BinaryOperatorExpression.Create(start, binop.End - start, exp, binop, ParseExpression()); //Question... this representation is slightly flawed if "operator precedence" matters... but does it matter? What scenario from a tooling perspective cares about precedence?
            }
            else
            {
                return exp;
            }
        }

        private ElseBlockNode ParseElseBlock()
        {
            if (!ParseExpected(TokenType.ElseKeyword)) //Question @cyrus... should I do this to validate all parse methods... when should a parse method return null?
            {
                return null;
            }

            var node = ElseBlockNode.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ElseKeyword = currentToken;
            node.Block = ParseBlock(Context.ElseBlock)?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ImmutableList<ElseIfBlockNode> ParseElseIfList()
        {
            var elseIfList = new List<ElseIfBlockNode>();

            while (ParseExpected(TokenType.ElseIfKeyword))
            {
                elseIfList.Add(ParseElseIfBlock());
            }

            return elseIfList.ToImmutableList();
        }

        private ElseIfBlockNode ParseElseIfBlock()
        {
            if (!ParseExpected(TokenType.ElseIfKeyword))
            {
                return null;
            }
            var node = ElseIfBlockNode.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ElseIfKeyword = currentToken;
            node.Exp = ParseExpression()?.ToBuilder();
            node.ThenKeyword = GetExpectedToken(TokenType.ThenKeyword);
            node.Block = ParseBlock(Context.ElseIfBlock)?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;

            return node.ToImmutable();
        }

        private PrefixExp ParsePrefixExp()
        {
            switch (Peek().Type)
            {
                case TokenType.OpenParen:
                    return ParseParenPrefixExp();
                case TokenType.Identifier:
                    switch (Peek(2).Type)
                    {
                        case TokenType.OpenCurlyBrace:
                        case TokenType.OpenParen:
                        case TokenType.String:
                        case TokenType.Colon:
                            return ParseFunctionCallExp();
                        case TokenType.Dot:
                            return ParseDotVar();
                        case TokenType.OpenBracket:
                            return ParseSquareBracketVar();
                        default:
                            return ParseNameVar();
                    }
                default:
                    return null; //Question @cyrus: correct?
            }
        }

        private SquareBracketVar ParseSquareBracketVar()
        {
            var bracketVarBuilder = SquareBracketVar.CreateBuilder();
            bracketVarBuilder.StartPosition = Peek().Start;
            bracketVarBuilder.PrefixExp = ParsePrefixExp()?.ToBuilder();
            bracketVarBuilder.OpenBracket = GetExpectedToken(TokenType.OpenBracket);
            bracketVarBuilder.Exp = ParseExpression()?.ToBuilder();
            bracketVarBuilder.CloseBracket = GetExpectedToken(TokenType.CloseBracket);
            bracketVarBuilder.Length = currentToken.End - bracketVarBuilder.StartPosition;
            return bracketVarBuilder.ToImmutable();
        }

        private DotVar ParseDotVar()
        {
            var dotVarBuilder = DotVar.CreateBuilder();
            dotVarBuilder.StartPosition = Peek().Start;
            dotVarBuilder.PrefixExp = ParseNameVar()?.ToBuilder();
            dotVarBuilder.DotOperator = GetExpectedToken(TokenType.Dot);
            dotVarBuilder.NameIdentifier = GetExpectedToken(TokenType.Identifier);
            dotVarBuilder.Length = currentToken.End - dotVarBuilder.StartPosition;
            return dotVarBuilder.ToImmutable();
        }

        private FunctionCallExp ParseFunctionCallExp()
        {
            var node = FunctionCallExp.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParseNameVar()?.ToBuilder();

            if(ParseExpected(TokenType.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(TokenType.Identifier);
            }

            node.Args = ParseArgs()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FunctionCallStatementNode ParseFunctionCallStatementNode()
        {
            var node = FunctionCallStatementNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.PrefixExp = ParseNameVar()?.ToBuilder();

            if (ParseExpected(TokenType.Colon))
            {
                node.Colon = currentToken;
                node.Name = GetExpectedToken(TokenType.Identifier);
            }

            node.Args = ParseArgs()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private Args ParseArgs()
        {
            switch (Peek().Type)
            {
                case TokenType.OpenParen:
                    return ParseParenArg();
                case TokenType.OpenCurlyBrace:
                    return ParseTableConstructorArg();
                case TokenType.String:
                    return ParseStringArg();
                default:
                    return null;
            }
        }

        private StringArg ParseStringArg()
        {
            var node = StringArg.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.StringLiteral = GetExpectedToken(TokenType.String);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableContructorArg ParseTableConstructorArg()
        {
            var node = TableContructorArg.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.OpenCurly = GetExpectedToken(TokenType.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(TokenType.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ParenArg ParseParenArg()
        {
            var node = ParenArg.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenParen = GetExpectedToken(TokenType.OpenParen);
            node.ExpList = ParseExpList()?.ToBuilder();
            node.CloseParen = GetExpectedToken(TokenType.CloseParen);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private NameVar ParseNameVar()
        {
            if (ParseExpected(TokenType.Identifier))
            {
                var node = NameVar.CreateBuilder();
                node.StartPosition = currentToken.Start;
                node.Name = currentToken;
                node.Length = currentToken.End - node.StartPosition;
                return node.ToImmutable();
            }
            else
            {
                return null;
            }
        }

        private ReturnStatementNode ParseRetStat()
        {
            if (ParseExpected(TokenType.ReturnKeyword))
            {
                return null;
            }

            var node = ReturnStatementNode.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.ReturnKeyword = currentToken;
            node.ReturnExpressions = ParseExpList()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructorNode ParseTableConstructor()
        {
            var node = TableConstructorNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(TokenType.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(TokenType.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private TableConstructorExp ParseTableConstructorExp()
        {
            var node = TableConstructorExp.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenCurly = GetExpectedToken(TokenType.OpenCurlyBrace);
            node.FieldList = ParseFieldList()?.ToBuilder();
            node.CloseCurly = GetExpectedToken(TokenType.CloseCurlyBrace);
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private FieldList ParseFieldList()
        {
            var node = FieldList.CreateBuilder();
            node.StartPosition = Peek().Start;
            bool parseFields = true;
            var fields = new List<FieldAndSeperatorPair>();

            while (parseFields)
            {
                var fieldAndSep = FieldAndSeperatorPair.CreateBuilder();
                fieldAndSep.Field = ParseField()?.ToBuilder();
                if (ParseExpected(TokenType.Comma) || ParseExpected(TokenType.Semicolon))
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

        private FieldNode ParseField()
        {
            switch (Peek().Type)
            {
                case TokenType.OpenBracket:
                    return ParseBracketField();
                case TokenType.Identifier:
                    if (Peek(2).Type == TokenType.AssignmentOperator)
                    {
                        var node = AssignmentField.CreateBuilder();
                        node.StartPosition = Peek().Start;
                        node.Name = GetExpectedToken(TokenType.Identifier);
                        node.AssignmentOperator = GetExpectedToken(TokenType.AssignmentOperator);
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
            node.StartPosition = currentToken.Start;
            node.OpenBracket = GetExpectedToken(TokenType.OpenBracket);
            node.IdentifierExp = ParseExpression()?.ToBuilder();
            node.CloseBracket = GetExpectedToken(TokenType.CloseBracket);
            node.AssignedExp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ExpField ParseExpField()
        {
            var node = ExpField.CreateBuilder();
            node.StartPosition = currentToken.End;
            node.Exp = ParseExpression()?.ToBuilder();
            node.Length = currentToken.End - node.StartPosition;
            return node.ToImmutable();
        }

        private ParenPrefixExp ParseParenPrefixExp()
        {
            var node = ParenPrefixExp.CreateBuilder();
            node.StartPosition = currentToken.Start;
            node.OpenParen = GetExpectedToken(TokenType.OpenParen);
            node.Exp = ParseExpression()?.ToBuilder();
            node.CloseParen = GetExpectedToken(TokenType.CloseParen);
            node.Length = currentToken.End;
            return node.ToImmutable();
        }

        private FunctionDef ParseFunctionDef()
        {
            var node = FunctionDef.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.FunctionKeyword = GetExpectedToken(TokenType.FunctionKeyword);
            node.FunctionBody = ParseFunctionBody()?.ToBuilder();
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private FuncBodyNode ParseFunctionBody()
        {
            var node = FuncBodyNode.CreateBuilder();
            node.StartPosition = Peek().Start;
            node.OpenParen = GetExpectedToken(TokenType.OpenParen);
            node.ParameterList = ParseParList()?.ToBuilder();
            node.CloseParen = GetExpectedToken(TokenType.CloseParen);
            node.Block = ParseBlock(Context.FuncBodyContext)?.ToBuilder();
            node.EndKeyword = GetExpectedToken(TokenType.EndKeyword);
            node.Length = Peek().FullStart - node.StartPosition - 1;
            return node.ToImmutable();
        }

        private ParList ParseParList()
        {
            if (Peek().Type == TokenType.VarArgOperator)
            {
                var node = VarArgPar.CreateBuilder();
                node.StartPosition = Peek().Start;
                node.VarargOperator = GetExpectedToken(TokenType.VarArgOperator);
                node.Length = currentToken.End - node.StartPosition;
                return node.ToImmutable();
            }
            else
            {
                var node = NameListPar.CreateBuilder();
                node.StartPosition = Peek().Start;
                node.NamesList = ParseNamesList()?.ToBuilder();
                if (ParseExpected(TokenType.Comma))
                {
                    if (Peek().Type == TokenType.VarArgOperator)
                    {
                        node.VarArgPar = CommaVarArgPair.Create(currentToken, NextToken())?.ToBuilder();
                    }
                    else
                    {
                        node.VarArgPar = CommaVarArgPair.Create(currentToken, null)?.ToBuilder();
                    }
                }
                node.Length = currentToken.End - node.StartPosition;
                return node.ToImmutable();
            }
        }

        private NameList ParseNamesList()
        {
            if (ParseExpected(TokenType.Identifier))
            {
                contextStack.Push(Context.NameListContext);

                var node = NameList.CreateBuilder();
                node.StartPosition = Peek().Start;

                List<NameCommaPair> names = new List<NameCommaPair>();

                //Add initial mandatory name with no preceding comma
                names.Add(NameCommaPair.Create(null, currentToken));

                while (ParseExpected(TokenType.Comma))
                {
                    names.Add(NameCommaPair.Create(currentToken, GetExpectedToken(TokenType.Identifier)));
                }

                node.Names = names.ToImmutableList();
                node.Length = currentToken.End - node.StartPosition;

                contextStack.Pop();
                return node.ToImmutable();
            }
            else
            {
                return null;
            }
        }

        private ExpList ParseExpList()
        {
            contextStack.Push(Context.ExpListContext);

            var node = ExpList.CreateBuilder();
            node.StartPosition = Peek().Start;

            List<ExpressionCommaPair> expressions = new List<ExpressionCommaPair>();

            ExpressionNode exp = ParseExpression();
            if (exp != null)
            {
                expressions.Add(ExpressionCommaPair.Create(null, exp));

                while (ParseExpected(TokenType.Comma))
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

        private Var ParsePotentialVarWithPrefixExp()
        {
            int tempPosition = positionInTokenList;
            ParsePrefixExp(); //Skip to the end of the prefix exp before checking.
            if (Peek().Type == TokenType.OpenBracket)
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