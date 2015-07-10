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
    }

    #region If Statement Nodes
    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ElseBlock : SyntaxNode
    {
        readonly Token elseKeyword;
        readonly Block block;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class ElseIfBlock
    { //TODO: file bug: inherit from syntax node
        readonly int startPosition;
        readonly int length;
        readonly Token elseIfKeyword;
        readonly Expression exp;
        readonly Token thenKeyword;
        readonly Block block;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class IfNode : SyntaxNode
    {
        readonly Token ifKeyword;
        readonly Expression exp;
        readonly Token thenKeyword;
        readonly Block ifBlock;
        readonly ImmutableList<ElseIfBlock> elseIfList;
        readonly ElseBlock elseBlock;
        readonly Token endKeyword;
    }
    #endregion

    #region Expression nodes
    [GenerateImmutable(GenerateBuilder = false)]
    public partial class Expression : SyntaxNode
    {
        ImmutableList<GenericExpression> expressions;

        public bool IsValidExpression()
        {
            //TODO: implement to check if there is a binop and then a missing exp or if there is a missing token or something
            return false;
        }
    }


    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class ConcreteExpression { }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class SimpleExpression : ConcreteExpression
    {
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
    public partial class ComplexExpression : ConcreteExpression
    {
        SyntaxNode expressionValue;

        public static bool IsValidExpressionNode(SyntaxNode node)
        {
            return (node is FunctionDef || node is PrefixExp || node is TableConstructor);
        }
    }
    #endregion

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
        //TODO: implement
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class FuncBody : SyntaxNode
    {
        Token openParen;
        ParList parameterList;
        Token closeParen;
        Block block;
        Token endKeyword;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public abstract partial class ParList : SyntaxNode { }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class VarArgPar : ParList
    {
        Token varargOperator;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class NameListPar : ParList
    {
        NameList names;
    }

    [GenerateImmutable(GenerateBuilder = false)]
    public partial class NameList : SyntaxNode
    {
        ImmutableList<Tuple<Token,Token>> names;
    }
}
