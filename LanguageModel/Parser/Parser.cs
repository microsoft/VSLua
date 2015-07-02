using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageModel
{
    class Parser
    {
        private List<ChunkNode> luaFiles;

        public Parser()
        {
            luaFiles = new List<ChunkNode>();
        }

        public void CreateNewParseTree(Stream stream)
        {
            IEnumerator<Token> tokenEnumerator = Lexer.Tokenize(stream).GetEnumerator();
            if(!tokenEnumerator.MoveNext())
            {
                //TODO: error?
            }
            luaFiles.Add(NodeCreator.ParseChunkNode(tokenEnumerator));
        }
    }
}
