using ImmutableObjectGraph;
using ImmutableObjectGraph.CodeGeneration;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LanguageService
{

    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class SyntaxNode : SyntaxNodeOrToken
    {
        [Required]
        private readonly SyntaxKind kind;
        [Required]
        readonly int startPosition;
        [Required]
        readonly int length;
        public override bool IsToken => false;
        public override bool IsLeafNode => this.Children.Count == 0;

        public IEnumerable<SyntaxNodeOrToken> Descendants()
        {
            if (this is SyntaxNode && ((SyntaxNode)this).kind == SyntaxKind.ChunkNode)
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

#region Simple Statement Nodes
#endregion
#region If Statement
#endregion
#region Expression nodes
#region FieldNodes
#endregion
#region PrefixExp Expression
#endregion
#endregion
#region Args Nodes
#endregion
#region List nodes
#endregion
}
