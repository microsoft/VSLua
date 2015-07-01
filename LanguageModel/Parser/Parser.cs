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
            var newProgramRootNode = ChunkNode.CreateBuilder();
            //newProgramRootNode.Parse(Lexer.Tokenize(stream).GetEnumerator(), TokenType.EndOfFile);
            //luaFiles.Add(newProgramRootNode);
        }
    }
}
