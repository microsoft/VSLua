namespace LanguageModel {
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
        ImmutableList<Trivia> triviaList;
        [Required]
        int fullStartPosition;
        [Required]
        int startPosition;
        [Required]
        int length;

        partial class Builder
        {
            public void ExtractTokenInfo(Token token)
            {
                this.FullStartPosition = token.FullStart;
                this.StartPosition = token.Start;
            }

            public void ExtractTokenInfoWithTrivia(Token token)
            {
                this.FullStartPosition = token.FullStart;
                this.StartPosition = token.Start;
                this.TriviaList = token.LeadingTrivia.ToImmutableList();
            }

            public SyntaxNode EnterNodeErrorState()
            {
                //TODO: implement?
                return null;
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Keyword : SyntaxNode
    {
        string value;
        partial class Builder
        {
            public void ExtractKeywordInfo(Token token)
            {
                this.FullStartPosition = token.FullStart;
                this.StartPosition = token.Start;
                this.Value = token.Text;
            }
        }
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Block : SyntaxNode
    {
        ImmutableList<SyntaxNode> children;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseBlock : SyntaxNode
    {
        Keyword elseKeyword;
        Block block;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ElseIfBlock
    { //TODO: inherit from syntax node
        Keyword elseIfKeyword;
        Expression exp;
        Keyword thenKeyword;
        Block block;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        Block programBlock;
        EndOfFileNode endOfFile;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class EndOfFileNode : SyntaxNode
    {
        //TODO: implement
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class IfNode : SyntaxNode
    {
        Keyword ifKeyword;
        Expression exp;
        Keyword thenKeyword;
        Block ifBlock;
        ImmutableList<ElseIfBlock> elseIfList;
        ElseBlock elseBlock;
        Keyword endKeyword;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class Expression : SyntaxNode
    {
        //TODO: implement further
        KeyValue keyvalue;
    }

    [GenerateImmutable(GenerateBuilder = true)]
    public partial class KeyValue : SyntaxNode
    {
        string value;
    }
}
