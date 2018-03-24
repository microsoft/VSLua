using ImmutableObjectGraph;
using ImmutableObjectGraph.CodeGeneration;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable SA1649 // File name must match first type name

namespace LanguageService
{
    public abstract class SyntaxNodeOrToken
    {
        public virtual bool IsLeafNode => true;
        public virtual bool IsToken => true;
        public abstract ImmutableList<SyntaxNodeOrToken> Children { get; }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class SyntaxNode : SyntaxNodeOrToken
    {
        [Required]
        private readonly SyntaxKind kind;
        [Required]
        private readonly int startPosition;
        [Required]
        private readonly int length;
        public override bool IsToken => false;
        public override bool IsLeafNode => this.Children.Count == 0;

        public IEnumerable<SyntaxNodeOrToken> Descendants()
        {
            if (this is SyntaxNode && ((SyntaxNode)this).Kind == SyntaxKind.ChunkNode)
            {
                yield return this;
            }

            foreach (var node in this.Children)
            {
                yield return node;

                var nodeAsSyntaxNode = node as SyntaxNode;

                if (nodeAsSyntaxNode != null)
                {
                    foreach (var nextNode in nodeAsSyntaxNode.Descendants())
                    {
                        yield return nextNode;
                    }
                }
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        private readonly BlockNode programBlock;
        [Required]
        private readonly Token endOfFile;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.programBlock, this.endOfFile);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BlockNode : SyntaxNode
    {
        [Required, NotRecursive]
        private readonly ImmutableList<StatementNode> statements;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return this.statements.Cast<SyntaxNodeOrToken>().ToImmutableList();
            }
        }
    }

    #region Simple Statement Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class StatementNode : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SemiColonStatementNode : StatementNode
    {
        [Required]
        private readonly Token semiColon;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.semiColon);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCallStatementNode : StatementNode
    {
        [Required]
        private readonly PrefixExp prefixExp;
        private readonly Token colon;
        private readonly Token name;
        [Required]
        private readonly Args args;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken>();
                children.Add(this.prefixExp);
                if (this.colon != null)
                {
                    children.Add(this.colon);
                    children.Add(this.name);
                }
                children.Add(this.args);
                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ReturnStatementNode : StatementNode
    {
        [Required]
        private readonly Token returnKeyword;
        [Required]
        private readonly SeparatedList expList;
        private readonly Token semiColon;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.returnKeyword, this.expList };
                if (this.semiColon != null)
                {
                    children.Add(this.semiColon);
                }

                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BreakStatementNode : StatementNode
    {
        [Required]
        private readonly Token breakKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.breakKeyword);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class GoToStatementNode : StatementNode
    {
        [Required]
        private readonly Token goToKeyword;
        [Required]
        private readonly Token name;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.goToKeyword, this.name);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class DoStatementNode : StatementNode
    {
        [Required]
        private readonly Token doKeyword;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token endKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.doKeyword, this.block, this.endKeyword);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class WhileStatementNode : StatementNode
    {
        [Required]
        private readonly Token whileKeyword;
        [Required]
        private readonly ExpressionNode exp;
        [Required]
        private readonly Token doKeyword;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token endKeyword;
        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.whileKeyword, this.exp, this.doKeyword, this.block, this.endKeyword);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class RepeatStatementNode : StatementNode
    {
        [Required]
        private readonly Token repeatKeyword;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token untilKeyword;
        [Required]
        private readonly ExpressionNode exp;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.repeatKeyword, this.block, this.untilKeyword, this.exp);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class GlobalFunctionStatementNode : StatementNode
    {
        [Required]
        private readonly Token functionKeyword;
        [Required]
        private readonly FuncNameNode funcName;
        [Required]
        private readonly FuncBodyNode funcBody;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.functionKeyword, this.funcName, this.funcBody);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LocalAssignmentStatementNode : StatementNode
    {
        [Required]
        private readonly Token localKeyword;
        [Required]
        private readonly SeparatedList nameList;
        private readonly Token assignmentOperator;
        private readonly SeparatedList expList;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken>();
                children.Add(this.localKeyword);
                children.Add(this.nameList);
                if (this.assignmentOperator != null)
                {
                    children.Add(this.assignmentOperator);
                    children.Add(this.expList);
                }
                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LocalFunctionStatementNode : StatementNode
    {
        [Required]
        private readonly Token localKeyword;
        [Required]
        private readonly Token functionKeyword;
        [Required]
        private readonly Token name;
        [Required]
        private readonly FuncBodyNode funcBody;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.localKeyword, this.functionKeyword, this.name, this.funcBody);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleForStatementNode : StatementNode
    {
        [Required]
        private readonly Token forKeyword;
        [Required]
        private readonly Token name;
        [Required]
        private readonly Token assignmentOperator;
        [Required]
        private readonly ExpressionNode exp1;
        [Required]
        private readonly Token comma;
        [Required]
        private readonly ExpressionNode exp2;
        private readonly Token optionalComma;
        private readonly ExpressionNode optionalExp3;
        [Required]
        private readonly Token doKeyword;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token endKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.forKeyword, this.name, this.assignmentOperator, this.exp1, this.comma, this.exp2 };
                if (this.optionalComma != null)
                {
                    children.Add(this.optionalComma);
                    children.Add(this.OptionalExp3);
                }
                children.Add(this.doKeyword);
                children.Add(this.block);
                children.Add(this.endKeyword);
                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MultipleArgForStatementNode : StatementNode
    {
        [Required]
        private readonly Token forKeyword;
        [Required]
        private readonly SeparatedList nameList;
        [Required]
        private readonly Token inKeyword;
        [Required]
        private readonly SeparatedList expList;
        [Required]
        private readonly Token doKeyword;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token endKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.forKeyword, this.nameList, this.inKeyword, this.expList, this.doKeyword, this.block, this.endKeyword);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class LabelStatementNode : StatementNode
    {
        [Required]
        private readonly Token doubleColon1;
        [Required]
        private readonly Token name;
        [Required]
        private readonly Token doubleColon2;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.doubleColon1, this.name, this.doubleColon2);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class AssignmentStatementNode : StatementNode
    {
        [Required]
        private readonly SeparatedList varList;
        [Required]
        private readonly Token assignmentOperator;
        [Required]
        private readonly SeparatedList expList;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.varList, this.assignmentOperator, this.expList);
            }
        }
    }
    #endregion

    #region If Statement
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class IfStatementNode : StatementNode
    {
        [Required]
        private readonly Token ifKeyword;
        [Required]
        private readonly ExpressionNode exp;
        [Required]
        private readonly Token thenKeyword;
        [Required]
        private readonly BlockNode ifBlock;
        [Required, NotRecursive]
        private readonly ImmutableList<ElseIfBlockNode> elseIfList;
        private readonly ElseBlockNode elseBlock;
        [Required]
        private readonly Token endKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.ifKeyword, this.exp, this.thenKeyword, this.ifBlock };

                //TODO remove temporary code:
                if (this.elseIfList != null)
                {
                    foreach (var node in this.elseIfList)
                    {
                        children.Add(node);
                    }
                }

                if (this.elseBlock != null)
                {
                    children.Add(this.elseBlock);
                }

                children.Add(this.endKeyword); //Why doesnt this do anything??!?!!
                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseBlockNode : SyntaxNode
    {
        [Required]
        private readonly Token elseKeyword;
        [Required]
        private readonly BlockNode block;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.elseKeyword, this.block);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseIfBlockNode : SyntaxNode
    {
        [Required]
        private readonly Token elseIfKeyword;
        [Required]
        private readonly ExpressionNode exp;
        [Required]
        private readonly Token thenKeyword;
        [Required]
        private readonly BlockNode block;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.elseIfKeyword, this.exp, this.thenKeyword, this.block);
            }
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
        private readonly Token expressionValue;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.expressionValue);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BinaryOperatorExpression : ExpressionNode
    {
        [Required]
        private readonly ExpressionNode exp1;
        [Required]
        private readonly Token binaryOperator;
        [Required]
        private readonly ExpressionNode exp2;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.exp1, this.binaryOperator, this.exp2);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class UnaryOperatorExpression : ExpressionNode
    {
        [Required]
        private readonly Token unaryOperator;
        [Required]
        private readonly ExpressionNode exp;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.unaryOperator, this.exp);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorExp : ExpressionNode
    {
        [Required]
        private readonly Token openCurly;
        [Required]
        private readonly SeparatedList fieldList;
        [Required]
        private readonly Token closeCurly;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openCurly, this.fieldList, this.closeCurly);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionDef : ExpressionNode
    {
        [Required]
        private readonly Token functionKeyword;
        [Required]
        private readonly FuncBodyNode functionBody;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.functionKeyword, this.functionBody);
            }
        }
    }

    #region FieldNodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class FieldNode : ExpressionNode { } //TODO: is this inheritance okay?

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BracketField : FieldNode
    {
        [Required]
        private readonly Token openBracket;
        [Required]
        private readonly ExpressionNode identifierExp;
        [Required]
        private readonly Token closeBracket;
        [Required]
        private readonly Token assignmentOperator;
        [Required]
        private readonly ExpressionNode assignedExp;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openBracket, this.identifierExp, this.closeBracket, this.assignmentOperator, this.assignedExp);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class AssignmentField : FieldNode
    {
        [Required]
        private readonly Token name;
        [Required]
        private readonly Token assignmentOperator;
        [Required]
        private readonly ExpressionNode exp;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.name, this.assignmentOperator, this.exp);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpField : FieldNode
    {
        [Required]
        private readonly ExpressionNode exp;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.exp);
            }
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
        private readonly Token name;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.name);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SquareBracketVar : Var
    {
        [Required]
        private readonly PrefixExp prefixExp;
        [Required]
        private readonly Token openBracket;
        [Required]
        private readonly ExpressionNode exp;
        [Required]
        private readonly Token closeBracket;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.prefixExp, this.openBracket, this.exp, this.closeBracket);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class DotVar : Var
    {
        [Required]
        private readonly PrefixExp prefixExp;
        [Required]
        private readonly Token dotOperator;
        [Required]
        private readonly Token nameIdentifier;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.prefixExp, this.dotOperator, this.nameIdentifier);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCallPrefixexp : PrefixExp
    {
        [Required]
        private readonly PrefixExp prefixExp;
        private readonly Token colon;
        private readonly Token name;
        [Required]
        private readonly Args args;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.prefixExp };
                if (this.colon != null)
                {
                    children.Add(this.colon);
                    children.Add(this.name);
                }
                children.Add(this.args);
                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenPrefixExp : PrefixExp
    {
        [Required]
        private readonly Token openParen;
        [Required]
        private readonly ExpressionNode exp;
        [Required]
        private readonly Token closeParen;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openParen, this.exp, this.closeParen);
            }
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
        private readonly Token openCurly;
        [Required]
        private readonly SeparatedList fieldList;
        [Required]
        private readonly Token closeCurly;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openCurly, this.fieldList, this.closeCurly);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenArg : Args
    {
        [Required]
        private readonly Token openParen;
        [Required]
        private readonly SeparatedList expList;
        [Required]
        private readonly Token closeParen;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openParen, this.expList, this.closeParen);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class StringArg : Args
    {
        [Required]
        private readonly Token stringLiteral;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.stringLiteral);
            }
        }
    }
    #endregion

    #region List nodes

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SeparatedList : SyntaxNode
    {
        [Required, NotRecursive]
        private readonly ImmutableList<SeparatedListElement> syntaxList;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken>();

                foreach (var listItem in this.syntaxList)
                {
                    foreach (var child in listItem.Children)
                    {
                        children.Add(child);
                    }
                }

                return children.ToImmutableList();
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SeparatedListElement : SyntaxNode
    {
        [Required]
        private readonly SyntaxNodeOrToken element;
        [Required]
        private readonly Token seperator;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                if (this.seperator != null)
                {
                    return ImmutableList.Create<SyntaxNodeOrToken>(this.element, this.seperator);
                }
                else
                {
                    return ImmutableList.Create<SyntaxNodeOrToken>(this.element);
                }
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParList : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class VarArgParList : ParList
    {
        [Required]
        private readonly Token varargOperator;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.varargOperator);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameListPar : ParList
    {
        [Required]
        private readonly SeparatedList namesList;
        private readonly Token comma;
        private readonly Token vararg;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.namesList };
                if (this.comma != null)
                {
                    children.Add(this.comma);
                    children.Add(this.vararg);
                }
                return children.ToImmutableList();
            }
        }
    }

    #endregion

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorNode : SyntaxNode
    {
        [Required]
        private readonly Token openCurly;
        [Required]
        private readonly SeparatedList fieldList;
        [Required]
        private readonly Token closeCurly;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openCurly, this.fieldList, this.closeCurly);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FuncBodyNode : SyntaxNode
    {
        [Required]
        private readonly Token openParen;
        [Required]
        private readonly ParList parameterList;
        [Required]
        private readonly Token closeParen;
        [Required]
        private readonly BlockNode block;
        [Required]
        private readonly Token endKeyword;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(this.openParen, this.parameterList, this.closeParen, this.block, this.endKeyword);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FuncNameNode : SyntaxNode
    {
        [Required]
        private readonly Token name;
        [Required, NotRecursive]
        private readonly SeparatedList funcNameList;
        private readonly Token optionalColon;
        private readonly Token optionalName;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                var children = new List<SyntaxNodeOrToken> { this.name, this.funcNameList };
                if (this.optionalColon != null)
                {
                    children.Add(this.optionalColon);
                    children.Add(this.optionalName);
                }
                return children.ToImmutableList();
            }
        }
    }
}
