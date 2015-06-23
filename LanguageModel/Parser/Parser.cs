using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    class Parser
    {
        private Lexer lexer;
        //private Token currentToken;
        //private List<Token> tokenList;
        //private int positionInTokenList;
        private List<Syntax.ChunkNode> luaFiles;

        public Parser()
        {
            this.lexer = new Lexer();
            luaFiles = new List<Syntax.ChunkNode>();
        }

        public void CreateParseTree(MoveableStreamReader stream)
        {
            luaFiles.Add(new Syntax.ChunkNode(lexer.Tokenize(stream))); //TODO: deal with lexer objects still tied to tokens from a previous list without erasing?

        }

        //private bool GetNextToken() ///TODO consider returning bool here...
        //{
        //    if (positionInTokenList < tokenList.Count)
        //    {
        //        currentToken = tokenList[positionInTokenList];
        //        positionInTokenList++;
        //        return true;
        //    }
        //    else
        //    {
        //        Console.WriteLine("ERROR: No more tokens to get!");
        //        return false;
        //    }
        //}

    }
}
