using ImmutableObjectGraph;
using ImmutableObjectGraph.CodeGeneration;
using LanguageService.LanguageModel.TreeVisitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace LanguageService
{

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class SyntaxNode : SyntaxNodeOrToken
    {
        [Required]
        readonly SyntaxKind kind;
        [Required]
        readonly int startPosition;
        [Required]
        readonly int length;

        public abstract void Accept(NodeWalker walker);
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        readonly BlockNode programBlock;
        [Required]
        readonly Token endOfFile;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(programBlock, endOfFile);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BlockNode : SyntaxNode
    {
        [Required]
        [NotRecursive]
        readonly ImmutableList<StatementNode> statements;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return statements.Cast<SyntaxNodeOrToken>().ToImmutableList();
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    #region Simple Statement Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class StatementNode : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SemiColonStatementNode : StatementNode
    {
        [Required]
        readonly Token semiColon;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(semiColon);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCallStatementNode : StatementNode
    {
        [Required]
        readonly PrefixExp prefixExp;
        readonly Token colon;
        readonly Token name;
        [Required]
        readonly Args args;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken>();
                children.Add(prefixExp);
                if (colon != null)
                {
                    children.Add(colon);
                    children.Add(name);
                }
                children.Add(args);
                return children.ToImmutableList();
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ReturnStatementNode : SyntaxNode
    {
        [Required]
        readonly Token returnKeyword;
        readonly ExpList returnExpressions;
        readonly Token semiColonRetStat;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(returnKeyword, returnExpressions);
                if (semiColonRetStat != null)
                    children.Add(semiColonRetStat);
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BreakStatementNode : StatementNode
    {
        [Required]
        readonly Token breakKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(breakKeyword);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class GoToStatementNode : StatementNode
    {
        [Required]
        readonly Token goToKeyword;
        [Required]
        readonly Token name;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(goToKeyword, name);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class DoStatementNode : StatementNode
    {
        [Required]
        readonly Token doKeyword;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token endKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(doKeyword, block, endKeyword);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class WhileStatementNode : StatementNode
    {
        [Required]
        readonly Token whileKeyword;
        [Required]
        readonly ExpressionNode exp;
        [Required]
        readonly Token doKeyword;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token endKeyword;
        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(whileKeyword, exp, doKeyword, block, endKeyword);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class RepeatStatementNode : StatementNode
    {
        [Required]
        readonly Token repeatKeyword;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token untilKeyword;
        [Required]
        readonly ExpressionNode exp;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(repeatKeyword, block, untilKeyword, exp);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class GlobalFunctionStatementNode : StatementNode
    {
        [Required]
        readonly Token functionKeyword;
        [Required]
        readonly FuncNameNode funcName;
        [Required]
        readonly FuncBodyNode funcBody;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(functionKeyword, funcName, funcBody);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LocalAssignmentStatementNode : StatementNode
    {
        [Required]
        readonly Token localKeyword;
        [Required]
        readonly NameList nameList;
        readonly Token assignmentOperator;
        readonly ExpList expList;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken>();
                children.Add(localKeyword);
                children.Add(nameList);
                if (assignmentOperator != null)
                {
                    children.Add(assignmentOperator);
                    children.Add(expList);
                }
                return children.ToImmutableList();
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LocalFunctionStatementNode : StatementNode
    {
        [Required]
        readonly Token localKeyword;
        [Required]
        readonly Token functionKeyword;
        [Required]
        readonly Token name;
        [Required]
        readonly FuncBodyNode funcBody;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(localKeyword, functionKeyword, name, funcBody);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleForStatementNode : StatementNode
    {
        [Required]
        readonly Token forKeyword;
        [Required]
        readonly Token name;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly ExpressionNode exp1;
        [Required]
        readonly Token comma;
        [Required]
        readonly ExpressionNode exp2;
        readonly Token optionalComma;
        readonly ExpressionNode optionalExp3;
        [Required]
        readonly Token doKeyword;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token endKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(forKeyword, name, assignmentOperator, exp1, comma, exp2);
                if (optionalComma != null)
                {
                    children.Add(optionalComma);
                    children.Add(OptionalExp3);
                }
                children.Add(doKeyword);
                children.Add(block);
                children.Add(endKeyword);
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MultipleArgForStatementNode : StatementNode
    {
        [Required]
        readonly Token forKeyword;
        [Required]
        readonly NameList nameList;
        [Required]
        readonly Token inKeyword;
        [Required]
        readonly ExpList expList;
        [Required]
        readonly Token doKeyword;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token endKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(forKeyword, nameList, inKeyword, expList, doKeyword, block, endKeyword);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LabelStatementNode : StatementNode
    {
        [Required]
        readonly Token doubleColon1;
        [Required]
        readonly Token name;
        [Required]
        readonly Token doubleColon2;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(doubleColon1, name, doubleColon2);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class AssignmentStatementNode : StatementNode
    {
        [Required]
        readonly VarList varList;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly ExpList expList;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(varList, assignmentOperator, expList);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
    #endregion

    #region If Statement
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class IfStatementNode : StatementNode
    {
        [Required]
        readonly Token ifKeyword;
        [Required]
        readonly ExpressionNode exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly BlockNode ifBlock;
        [Required, NotRecursive]
        readonly ImmutableList<ElseIfBlockNode> elseIfList;
        readonly ElseBlockNode elseBlock;
        [Required]
        readonly Token endKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(ifKeyword, exp, thenKeyword, ifBlock);
                foreach (var node in elseIfList)
                {
                    children.Add(node);
                }
                children.Add(elseBlock);
                children.Add(endKeyword);
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseBlockNode : SyntaxNode
    {
        [Required]
        readonly Token elseKeyword;
        [Required]
        readonly BlockNode block;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(elseKeyword, block);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseIfBlockNode : SyntaxNode
    {
        [Required]
        readonly Token elseIfKeyword;
        [Required]
        readonly ExpressionNode exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly BlockNode block;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(elseIfKeyword, exp, thenKeyword, block);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
    #endregion

    #region Expression nodes

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ExpressionNode : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleExpression : ExpressionNode
    {
        [Required]
        readonly Token expressionValue;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(expressionValue);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BinaryOperatorExpression : ExpressionNode
    {
        [Required]
        readonly ExpressionNode exp1;
        [Required]
        readonly Token binaryOperator;
        [Required]
        readonly ExpressionNode exp2;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(exp1, binaryOperator, exp2);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class UnaryOperatorExpression : ExpressionNode
    {
        [Required]
        readonly Token unaryOperator;
        [Required]
        readonly ExpressionNode exp;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(unaryOperator, exp);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorExp : ExpressionNode
    {
        [Required]
        readonly Token openCurly;
        [Required]
        FieldList fieldList;
        [Required]
        readonly Token closeCurly;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(openCurly, fieldList, closeCurly);
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionDef : ExpressionNode
    {
        [Required]
        readonly Token functionKeyword;
        [Required]
        readonly FuncBodyNode functionBody;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(functionKeyword, functionBody);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    #region FieldNodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class FieldNode : ExpressionNode { } //TODO: is this inheritance okay?

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BracketField : FieldNode
    {
        [Required]
        readonly Token openBracket;
        [Required]
        readonly ExpressionNode identifierExp;
        [Required]
        readonly Token closeBracket;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly ExpressionNode assignedExp;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openBracket, identifierExp, closeBracket, assignmentOperator, assignedExp);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class AssignmentField : FieldNode
    {
        [Required]
        readonly Token name;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly ExpressionNode exp;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(name, assignmentOperator, exp);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpField : FieldNode
    {
        [Required]
        readonly ExpressionNode exp;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(exp);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
    #endregion

    #region PrefixExp Expression
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class PrefixExp : ExpressionNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class Var : PrefixExp { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameVar : Var
    {
        [Required]
        readonly Token name;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(name);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SquareBracketVar : Var
    {
        [Required]
        readonly PrefixExp prefixExp;
        [Required]
        readonly Token openBracket;
        [Required]
        readonly ExpressionNode exp;
        [Required]
        readonly Token closeBracket;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(prefixExp, openBracket, exp, closeBracket);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class DotVar : Var
    {
        [Required]
        readonly PrefixExp prefixExp;
        [Required]
        readonly Token dotOperator;
        [Required]
        readonly Token nameIdentifier;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(prefixExp, dotOperator, nameIdentifier);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCallExp : PrefixExp
    {
        [Required]
        readonly PrefixExp prefixExp;
        readonly Token colon;
        readonly Token name;
        [Required]
        readonly Args args;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(prefixExp);
                if (colon != null)
                {
                    children.Add(colon);
                    children.Add(name);
                }
                children.Add(args);
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenPrefixExp : PrefixExp
    {
        [Required]
        readonly Token openParen;
        [Required]
        readonly ExpressionNode exp;
        [Required]
        readonly Token closeParen;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openParen, exp, closeParen);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
    #endregion

    #endregion

    #region Args Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class Args : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableContructorArg : Args
    {
        [Required]
        readonly Token openCurly;
        [Required]
        readonly FieldList fieldList;
        [Required]
        readonly Token closeCurly;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openCurly, fieldList, closeCurly);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            throw new NotImplementedException();
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenArg : Args
    {
        [Required]
        readonly Token openParen;
        [Required]
        readonly ExpList expList;
        [Required]
        readonly Token closeParen;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openParen, expList, closeParen);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            throw new NotImplementedException();
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class StringArg : Args
    {
        [Required]
        readonly Token stringLiteral;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(stringLiteral);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
    #endregion

    #region List nodes

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SeparatedList : SyntaxNode
    {
        [Required, NotRecursive]
        readonly ImmutableList<SeparatedListElement> syntaxList;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return syntaxList.Cast<SyntaxNodeOrToken>().ToImmutableList();
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SeparatedListElement : SyntaxNode
    {
        [Required]
        readonly Token seperator;
        [Required]
        readonly SyntaxNodeOrToken element;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(seperator, element);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    #region Code To Deprecate
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<NameCommaPair> names;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<FieldAndSeperatorPair> fields;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<ExpressionCommaPair> expressions;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParList : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class VarArgPar : ParList
    {
        [Required]
        readonly Token varargOperator;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameListPar : ParList
    {
        [Required]
        readonly NameList namesList;
        readonly Token comma;
        readonly Token vararg;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class VarList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<CommaVarPair> vars;

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    #region List Pairs
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class CommaVarPair
    {
        [Required]
        readonly Token comma;
        [Required]
        readonly Var var;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class CommaVarArgPair
    {
        [Required]
        readonly Token comma;
        [Required]
        readonly Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameCommaPair
    {
        [Required]
        readonly Token comma;
        [Required]
        readonly Token name;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpressionCommaPair
    {
        [Required]
        readonly Token comma;
        [Required]
        readonly ExpressionNode expression;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldAndSeperatorPair
    {
        readonly FieldNode field;
        readonly Token fieldSeparator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameDotPair
    {
        [Required]
        readonly Token dot;
        [Required]
        readonly Token name;
    }
    #endregion
    #endregion
    #endregion

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorNode : SyntaxNode
    {
        [Required]
        readonly Token openCurly;
        [Required]
        readonly FieldList fieldList;
        [Required]
        readonly Token closeCurly;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openCurly, fieldList, closeCurly);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FuncBodyNode : SyntaxNode
    {
        [Required]
        readonly Token openParen;
        [Required]
        readonly ParList parameterList;
        [Required]
        readonly Token closeParen;
        [Required]
        readonly BlockNode block;
        [Required]
        readonly Token endKeyword;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(openParen, parameterList, closeParen, block, endKeyword);
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FuncNameNode : SyntaxNode
    {
        [Required]
        readonly Token name;
        [Required, NotRecursive]
        readonly SeparatedList funcNameList;
        readonly Token optionalColon;
        readonly Token optionalName;

        public ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = ImmutableList.Create<SyntaxNodeOrToken>(name, funcNameList);
                if(optionalColon!=null)
                {
                    children.Add(optionalColon);
                    children.Add(optionalName);
                }
                return children;
            }
        }

        public override void Accept(NodeWalker walker)
        {
            walker.Visit(this);
        }
    }
}
