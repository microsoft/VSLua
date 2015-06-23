using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Syntax
{
    internal class ChunkNode : SyntaxNode
    {
        private Block programBlock;
        private EndOfFileNode EndOfFile;

        public ChunkNode(List<Token> tokenList) : base()
        {
            //tokenList.to
            //programBlock = new Block(tokenList);
            //EndOfFile = new EndOfFileNode(tokenList);
        }
    }
}
