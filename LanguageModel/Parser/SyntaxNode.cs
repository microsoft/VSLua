using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    internal class SyntaxNode
    {
        private int startPosition;
        private int endPosition;
        private List<SyntaxNode> ListofErrorNodes; //TODO: necessary?

        public SyntaxNode()
        {
        }

        public SyntaxNode(int start, int end)
        {
            this.startPosition = start;
            this.endPosition = end;
        }

        //private bool accept(Token token)
        //{

        //}

        //private bool expect(Token token)
        //{

        //}

        public void EnterErrorMode(SyntaxNode targetNode)
        {
            ListofErrorNodes = new List<SyntaxNode>();
            //TODO: add tokens to error list and stop once the target node/stop condition is reached
        }

    }
}
