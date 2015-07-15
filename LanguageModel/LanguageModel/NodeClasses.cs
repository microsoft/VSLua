namespace LanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ImmutableObjectGraph.CodeGeneration;

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SyntaxNode
    {
        [Required]
        int startPosition;
        [Required]
        int length;

        internal virtual void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Syntax Node");
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MissingNode : SyntaxNode
    {
        //TODO: add missing type
        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Missing Node");
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MisplacedToken : SyntaxNode
    {
        Token token;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Missing Token: " + token.ToString());
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        readonly Block programBlock;
        [Required]
        readonly Token endOfFile;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ChunkNode");
            using (indentingWriter.Indent())
            {
                programBlock.ToString(indentingWriter);
            }
            
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine(endOfFile.ToString());
            }

        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Block : SyntaxNode
    {
        [Required]
        ImmutableList<SyntaxNode> children;
        RetStat returnStatement;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Block");
            foreach (var child in this.children)
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
    public partial class IfNode : SyntaxNode
    {
        [Required]
        readonly Token ifKeyword;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly Block ifBlock;
        readonly ImmutableList<ElseIfBlock> elseIfList;
        readonly ElseBlock elseBlock;
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
                        block.ToString(indentingWriter);
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
    public partial class ElseBlock : SyntaxNode
    {
        [Required]
        readonly Token elseKeyword;
        [Required]
        readonly Block block;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseIfBlock
    { //TODO: inherit from syntax node once ImmutableGraphObject is bug is fixed
        [Required]
        readonly int startPosition;
        [Required]
        readonly int length;
        [Required]
        readonly Token elseIfKeyword;
        [Required]
        readonly Expression exp;
        [Required]
        readonly Token thenKeyword;
        [Required]
        readonly Block block;

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
        Token expressionValue;
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
    public partial class BinopExpression : Expression
    {
        [Required]
        Expression exp1;
        [Required]
        Token binop;
        [Required]
        Expression exp2;

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
                indentingWriter.WriteLine(binop.ToString());
            }
            using (indentingWriter.Indent())
            {
                exp2.ToString(indentingWriter);
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class UnopExpression : Expression
    {
        [Required]
        Token unop;
        [Required]
        Expression exp;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine(unop.ToString());
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
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;
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
        Token functionKeyword;
        [Required]
        FuncBody functionBody;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class PrefixExp : Expression { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Var : PrefixExp { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameVar : Var
    {
        [Required]
        Token identifier;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SquareBracketVar : Var
    {
        [Required]
        PrefixExp prefixExp;
        [Required]
        Token openBracket;
        [Required]
        Expression exp;
        [Required]
        Token closeBracket;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class DotVar : Var
    {
        [Required]
        PrefixExp prefixExp;
        [Required]
        Token dotOperator;
        [Required]
        Token nameIdentifier;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionCall : PrefixExp //TODO: bad practice? see reference manual
    {
        [Required]
        PrefixExp prefixExp;
        Token colon;
        Token name;
        [Required]
        Args args;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ParenPrefixExp : PrefixExp
    {
        [Required]
        Token openParen;
        [Required]
        Expression exp;
        [Required]
        Token closeParen;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("ParenPrefixExp");
            using (indentingWriter.Indent())
            {
                exp.ToString(indentingWriter);
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
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParenArg : Args
    {
        [Required]
        Token openParen;
        [Required]
        ExpList expList;
        [Required]
        Token closeParen;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class StringArg : Args
    {
        [Required]
        Token stringLiteral;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SemiColonStatement : SyntaxNode
    {
        Token token;

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine(token.ToString());
        }
    }

    #endregion

    #region List nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameList : SyntaxNode
    {
        [Required]
        ImmutableList<NameCommaPair> names;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldList : SyntaxNode
    {
        [Required]
        ImmutableList<FieldAndSeperatorPair> fields;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpList : SyntaxNode
    {
        [Required]
        ImmutableList<ExpressionCommaPair> expressions;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class ParList : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class VarArgPar : ParList
    {
        [Required]
        Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameListPar : ParList
    {
        [Required]
        NameList namesList;
        [Required]
        CommaVarArgPair varArgPar;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class CommaVarArgPair
    {
        [Required]
        Token comma;
        [Required]
        Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class NameCommaPair
    {
        [Required]
        Token name;
        [Required]
        Token comma;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpressionCommaPair
    {
        [Required]
        Token comma;
        [Required]
        Expression expression;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldAndSeperatorPair
    {
        Field field;
        Token fieldSeparator;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class Field : Expression { } //TODO: is this inheritance okay?

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BracketField : Field
    {
        [Required]
        Token openBracket;
        [Required]
        Expression identifierExp;
        [Required]
        Token closeBracket;
        [Required]
        Token assignmentOperator;
        [Required]
        Expression assignedExp;

        internal override void ToString(TextWriter indentingWriter)
        {
            throw new NotImplementedException();
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class SimpleField : Field
    {
        [Required]
        Token name;
        [Required]
        Token assignmentOperator;
        [Required]
        Expression exp;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ExpField : Field
    {
        [Required]
        Expression exp;
    }
    #endregion

    #region Other Nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class RetStat : SyntaxNode
    {
        [Required]
        Token returnKeyword;
        ExpList returnExpressions;
        //Token semiColonRetStat; TODO: decide if necessary?

        internal override void ToString(TextWriter writer)
        {
            var indentingWriter = IndentingTextWriter.Get(writer);
            indentingWriter.WriteLine("RetStat");
            using (indentingWriter.Indent())
            {
                indentingWriter.WriteLine("explist... implement"); //TODO
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructor : SyntaxNode
    {
        [Required]
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;

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
    public partial class FuncBody : SyntaxNode
    {
        [Required]
        Token openParen;
        [Required]
        ParList parameterList;
        [Required]
        Token closeParen;
        [Required]
        Block block;
        [Required]
        Token endKeyword;
    }
    #endregion

}
