using LanguageService;
using System.Collections.Generic;
using Xunit;

namespace LanguageModel.Tests.TestGeneration
{
    class Tester
    {
        private IEnumerator<SyntaxNodeOrToken> treeEnumerator;

        public Tester(SyntaxTree actualTree)
        {
            this.treeEnumerator = actualTree.Root.Descendants().GetEnumerator();
        }

        public void N(SyntaxKind kind)
        {
            treeEnumerator.MoveNext();
            //TODO remove is-check once Immutable graph object bug is fixed. 
            if (treeEnumerator.Current is SyntaxNode)
            {
                Assert.Equal(kind, ((SyntaxNode)treeEnumerator.Current).Kind);
            } else
            {
                Assert.Equal(kind, ((Token)treeEnumerator.Current).Kind);
            }
        }
    }
}
