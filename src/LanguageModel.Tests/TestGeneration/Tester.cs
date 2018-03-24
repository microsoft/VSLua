// Copyright (c) Microsoft. All rights reserved.

namespace LanguageModel.Tests.TestGeneration
{
    using System.Collections.Generic;
    using LanguageService;
    using Xunit;

    internal class Tester
    {
        private IEnumerator<SyntaxNodeOrToken> treeEnumerator;

        public Tester(SyntaxTree actualTree)
        {
            this.treeEnumerator = actualTree.Root.Descendants().GetEnumerator();
        }

        public void N(SyntaxKind kind)
        {
            this.treeEnumerator.MoveNext();
            //TODO remove is-check once Immutable graph object bug is fixed.
            if (this.treeEnumerator.Current is SyntaxNode)
            {
                Assert.Equal(kind, ((SyntaxNode)this.treeEnumerator.Current).Kind);
            }
            else
            {
                Assert.Equal(kind, ((Token)this.treeEnumerator.Current).Kind);
            }
        }
    }
}
