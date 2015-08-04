using LanguageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LanguageModel.Tests
{
    class TestGenerator
    {
        private SyntaxTree actualTree;

        TestGenerator(SyntaxTree actualTree)
        {
            this.actualTree = actualTree;
        }

        public void N(SyntaxKind kind)
        {
            //Assert.Equal(kind, actualTree.PreorderTraversal().Kind);
        }
    }
}
