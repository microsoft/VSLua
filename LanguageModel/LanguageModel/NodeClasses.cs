using ImmutableObjectGraph;
using ImmutableObjectGraph.CodeGeneration;
using System;
using System.Collections.Immutable;
using System.IO;

namespace LanguageService
{

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SyntaxNode
    {
        [Required]
        readonly int startPosition;
        [Required]
        readonly int length;

        internal virtual void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Syntax Node");
        }
    }


    [GenerateImmutable(GenerateBuilder = true)]
    public partial class StatementNode : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MisplacedTokenNode : StatementNode
    {
        [Required]
        readonly Token token;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Missing Token: " + token.ToString());
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SemiColonStatementNode : StatementNode
    {
        [Required]
        readonly Token token;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine(token.ToString());
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        readonly BlockNode programBlock;
        [Required]
        readonly Token endOfFile;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ChunkNode");
            using (indentingWriter.Indent())
            {
                programBlock.ToString(indentingWriter);
                indentingWriter.WriteLine(endOfFile.ToString());
            }

        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BlockNode : SyntaxNode
    {
        [Required]
        [NotRecursive]
        readonly ImmutableList<StatementNode> statements;
        readonly ReturnStatNode returnStatement;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Block");
            foreach (var child in this.statements)
            {
                using (indentingWriter.Indent())
                {
                    child.ToString(indentingWriter);
                }
            }

            if(returnStatement != null)
            {
                using (indentingWriter.Indent())
                {
                    returnStatement.ToString(indentingWriter);
                }
            }            
        }
    }

    #region If Statement Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class IfStatementNode : StatementNode
    {
        [Required]
        readonly Token ifKeyword;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly BlockNode ifBlock;
        [Required, NotRecursive]
        readonly ImmutableList<ElseIfBlockNode> elseIfList;
        readonly ElseBlockNode elseBlock;
        [Required]
        readonly Token endKeyword;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("IfNode");
            using (indentingWriter.Indent())
            {
                exp.ToString(indentingWriter);
            }

            using (indentingWriter.Indent())
            {
                ifBlock.ToString(indentingWriter);
            }

            if (elseIfList != null)
            {
                foreach (var block in elseIfList)
                {
                    using (indentingWriter.Indent())
                    {
                        if (block != null)
                        {
                            block.ToString(indentingWriter);
                        }
                        else
                        {
                            indentingWriter.WriteLine("null");
                      }
                    }
                }
            }

            if (elseBlock != null)
            {
                using (indentingWriter.Indent())
                {
                    elseBlock.ToString(indentingWriter);
                }
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseBlockNode : SyntaxNode
    {
        [Required]
        readonly Token elseKeyword;
        [Required]
        readonly BlockNode block;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ElseBlock");
            using (indentingWriter.Indent())
            {
                block.ToString(indentingWriter);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseIfBlockNode : SyntaxNode
    { 
        [Required]
        readonly Token elseIfKeyword;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly BlockNode block;

        internal void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ElseIfBlock: ");
            using (indentingWriter.Indent())
            {
                exp.ToString(indentingWriter);
            }

            using (indentingWriter.Indent())
            {
                block.ToString(indentingWriter);
            }
        }
    }
    #endregion

    #region Expression nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class Expression : SyntaxNode
    {
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleExpression : Expression
    {
        [Required]
        readonly Token expressionValue;
        public static bool IsValidExpressionNode(TokenType type)
        {
            switch (type)
            {
                case TokenType.Number:
                case TokenType.TrueKeyValue:
                case TokenType.FalseKeyValue:
                case TokenType.NilKeyValue:
                case TokenType.VarArgOperator:
                case TokenType.String:
                    return true;
                default:
                    return false;
            }
        }

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Expression:\t" + expressionValue.ToString());
        }

    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BinaryOperatorExpression : Expression
    {
        [Required]
        readonly Expression exp1;
        [Required]
        readonly Token binaryOperator;
        [Required]
        readonly Expression exp2;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                exp1.ToString(indentingWriter);
            }
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine(binaryOperator.ToString());
            }
            using (indentingWriter.Indent())
            {
                exp2.ToString(indentingWriter);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class UnaryOperatorExpression : Expression
    {
        [Required]
        readonly Token unaryOperator;
        [Required]
        readonly Expression exp;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine(unaryOperator.ToString());
            }
            using (indentingWriter.Indent())
            {
                exp.ToString(indentingWriter);
            }
        }
    }
    #endregion

    #region Other Expression Nodes (out of scope for Code review)
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorExp : Expression
    {
        [Required]
        readonly Token openCurly;
        FieldList fieldList;
        [Required]
        readonly Token closeCurly;
        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("TableConstructor");
            using (indentingWriter.Indent())
            {
                FieldList.ToString(indentingWriter);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionDef : Expression
    {
        [Required]
        readonly Token functionKeyword;
        [Required]
        readonly FuncBodyNode functionBody;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class PrefixExp : Expression { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Var : PrefixExp { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameVar : Var
    {
        [Required]
        readonly Token identifier;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SquareBracketVar : Var
    {
        [Required]
        readonly PrefixExp prefixExp;
        [Required]
        readonly Token openBracket;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token closeBracket;
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
    }
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCallStatement : StatementNode 
    {
        [Required]
        readonly PrefixExp prefixExp;
        readonly Token colon;
        readonly Token name;
        [Required]
        readonly Args args;
    }
    
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenPrefixExp : PrefixExp
    {
        [Required]
        readonly Token openParen;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token closeParen;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ParenPrefixExp");
            using (indentingWriter.Indent())
            {
                if (exp != null)
                {
                    exp.ToString(indentingWriter);
                } else
                {
                    indentingWriter.WriteLine("null");
                }
            }
        }
    }
    #endregion

    #region Args Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class Args : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class TableContructorArg : Args
    {
        [Required]
        readonly Token openCurly;
        readonly FieldList fieldList;
        [Required]
        readonly Token closeCurly;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParenArg : Args
    {
        [Required]
        readonly Token openParen;
        [Required]
        readonly ExpList expList;
        [Required]
        readonly Token closeParen;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class StringArg : Args
    {
        [Required]
        readonly Token stringLiteral;
    }
    #endregion

    #region List nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<NameCommaPair> names;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<FieldAndSeperatorPair> fields;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpList : SyntaxNode
    {
        [Required]
        readonly ImmutableList<ExpressionCommaPair> expressions;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParList : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class VarArgPar : ParList
    {
        [Required]
        readonly Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameListPar : ParList
    {
        [Required]
        readonly NameList namesList;
        [Required]
        readonly CommaVarArgPair varArgPar;
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
        readonly Token name;
        [Required]
        readonly Token comma;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpressionCommaPair
    {
        [Required]
        readonly Token comma;
        [Required]
        readonly Expression expression;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldAndSeperatorPair
    {
        readonly FieldNode field;
        readonly Token fieldSeparator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class FieldNode : Expression { } //TODO: is this inheritance okay?

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BracketField : FieldNode
    {
        [Required]
        readonly Token openBracket;
        [Required]
        readonly Expression identifierExp;
        [Required]
        readonly Token closeBracket;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly Expression assignedExp;

        internal override void ToString(TextWriter indentingWriter)
        {
            throw new NotImplementedException();
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleField : FieldNode
    {
        [Required]
        readonly Token name;
        [Required]
        readonly Token assignmentOperator;
        [Required]
        readonly Expression exp;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpField : FieldNode
    {
        [Required]
        readonly Expression exp;
    }
    #endregion

    #region Other Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ReturnStatNode : SyntaxNode
    {
        [Required]
        readonly Token returnKeyword;
        readonly ExpList returnExpressions;
        //Token semiColonRetStat; Question: is this really necessary even though defined in the language?

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ReturnStat");
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine("explist... implement"); //TODO: implement
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorNode : SyntaxNode
    {
        [Required]
        readonly Token openCurly;
        readonly FieldList fieldList;
        [Required]
        readonly Token closeCurly;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("TableConstructor");
            using (indentingWriter.Indent())
            {
                FieldList.ToString(indentingWriter);
            }
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
    }
    #endregion

}
