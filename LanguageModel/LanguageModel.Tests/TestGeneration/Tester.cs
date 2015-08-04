using LanguageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LanguageModel.Tests.TestGeneration
{
    class Tester
    {
        private IEnumerator<SyntaxNodeOrToken> treeEnumerator;

        public Tester(SyntaxTree actualTree)
        {
            this.treeEnumerator = actualTree.Next(actualTree.Root).GetEnumerator();
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
