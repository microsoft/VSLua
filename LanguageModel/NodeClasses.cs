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
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class MissingNode : SyntaxNode
    {
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        [Required]
        readonly Block programBlock;
        [Required]
        readonly Token endOfFile;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Block : SyntaxNode
    {
        [Required]
        ImmutableList<SyntaxNode> children;
        RetStat returnStatement;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class RetStat : SyntaxNode
    {
        [Required]
        Token returnKeyword;
        ExpList returnExpressions;
        //Token semiColonRetStat; TODO: decide if necessary?
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructor : SyntaxNode
    {
        [Required]
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;
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

    #region Args Classes
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

    #endregion

    #region Lua List nodes
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
        Expression expression;
        [Required]
        Token comma;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class FieldSep { }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class CommaFieldSep : FieldSep
    {
        Token comma;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class CollonFieldSep : FieldSep
    {
        Token colon;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FieldAndSeperatorPair
    {
        [Required]
        Token field;
        [Required]
        FieldSep fieldSeparator;
    }
    #endregion

    #region If Statement Nodes
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
    }

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
    }
    #endregion

    #region Expression nodes
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Expression : SyntaxNode { }

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
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ComplexExpression : Expression
    {
        [Required]
        SyntaxNode expressionValue;

        public static bool IsValidExpressionNode(SyntaxNode node)
        {
            return (node is FunctionDef || node is PrefixExp || node is TableConstructor);
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
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class UnopExpression : Expression
    {
        [Required]
        Token unop;
        [Required]
        Expression exp;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class TableConstructorExp : Expression
    {
        [Required]
        Token openCurly;
        FieldList fieldList;
        [Required]
        Token closeCurly;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class FunctionDef : SyntaxNode
    {
        Token functionKeyword;
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
    }
    #endregion

}
