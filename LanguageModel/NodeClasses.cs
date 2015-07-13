namespace LanguageModel
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


    [GenerateImmutable(GenerateBuilder = false)]
    public partial class SyntaxNode
    {
        [Required]
        int startPosition;
        [Required]
        int length;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        readonly Block programBlock;
        [Required]
        readonly Token endOfFile;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class Block : SyntaxNode
    {
        [Required]
        ImmutableList<SyntaxNode> children;
        RetStat returnStatement;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class RetStat : SyntaxNode
    {
        [Required]
        Token returnKeyword;
        ExpList returnExpressions;
        Token semiColonRetStat;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class FunctionDef : SyntaxNode
    {
        Token functionKeyword;
        FuncBody functionBody;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class PrefixExp : SyntaxNode
    {
        //TODO: implement
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class TableConstructor : SyntaxNode
    {
        [Required]
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;
    }

    [GenerateImmutable(GenerateBuilder = false)]
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

    #region Lua List nodes
    [GenerateImmutable(GenerateBuilder = false)]
    public partial class NameList
    {
        //TODO: Update to inherit from SyntaxNode once bug fixed in Immutable Graph Object
        [Required]
        int startPosition;
        [Required]
        int length;
        [Required]
        ImmutableList<NameCommaPair> names;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class FieldList : SyntaxNode
    {
        [Required]
        ImmutableList<FieldAndSeperatorPair> fields;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ExpList : SyntaxNode
    {
        [Required]
        ImmutableList<ExpressionCommaPair> expressions;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class ParList //TODO: Update to inherit from SyntaxNode once bug fixed in Immutable Graph Object
    {
        [Required]
        int startPosition;
        [Required]
        int length;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class VarArgPar : ParList
    {
        [Required]
        Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class NameListPar : ParList
    {
        [Required]
        NameList namesList;
        Token comma; //TODO: indicate that these are optional?
        Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class NameCommaPair
    {
        [Required]
        Token name;
        [Required]
        Token comma;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ExpressionCommaPair
    {
        [Required]
        Expression expression;
        [Required]
        Token comma;
    }    

    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class FieldSep { }

    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class CommaFieldSep : FieldSep
    {
        Token comma;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class CollonFieldSep : FieldSep
    {
        Token colon;
    }


    [GenerateImmutable(GenerateBuilder = false)]
    public partial class FieldAndSeperatorPair
    {
        [Required]
        Token field;
        [Required]
        FieldSep fieldSeparator;
    }
    #endregion

    #region If Statement Nodes
    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ElseBlock : SyntaxNode
    {
        [Required]
        readonly Token elseKeyword;
        [Required]
        readonly Block block;
    }

    [GenerateImmutable(GenerateBuilder = false)]
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
    }

    [GenerateImmutable(GenerateBuilder = false)]
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
    }
    #endregion

    #region Expression nodes
    [GenerateImmutable(GenerateBuilder = false)]
    public partial class Expression
    {
        //TODO: inherit from syntax node once ImmutableGraphObject is bug is fixed
        [Required]
        readonly int startPosition;
        [Required]
        readonly int length;
    }

    [GenerateImmutable(GenerateBuilder = false)]
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
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ComplexExpression : Expression
    {
        [Required]
        SyntaxNode expressionValue;

        public static bool IsValidExpressionNode(SyntaxNode node)
        {
            return (node is FunctionDef || node is PrefixExp || node is TableConstructor);
        }
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class BinopExpression : Expression
    {
        [Required]
        Expression exp1;
        [Required]
        Token binop;
        [Required]
        Expression exp2;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class UnopExpression : Expression
    {
        [Required]
        Token unop;
        [Required]
        Expression exp;
    }
    #endregion

}
