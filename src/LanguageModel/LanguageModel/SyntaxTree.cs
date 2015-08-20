using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace LanguageService
{
    public class SyntaxTree
    {
        public SyntaxTree(ChunkNode root, ImmutableList<ParseError> errorList)
        {
            this.Root = root;
            this.ErrorList = errorList;
        }

        public ChunkNode Root { get; }
        public ImmutableList<ParseError> ErrorList { get; }

        public static SyntaxTree Create(string filename)
        {
            TextReader luaStream = File.OpenText(filename);
            return Parser.Parse(luaStream);
        }

        public static SyntaxTree CreateFromString(string program)
        {
            TextReader luaStream = new StringReader(program);
            return Parser.Parse(luaStream);
        }

        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
        }
    }
}
