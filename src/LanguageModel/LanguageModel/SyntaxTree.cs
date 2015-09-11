using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace LanguageService
{
    public class SyntaxTree
    {
        public SyntaxTree(ChunkNode root, List<Token> tokens, List<StatementNode> statementNodeList, ImmutableList<ParseError> errorList)
        {
            this.Root = root;
            this.Tokens = tokens;
            this.ErrorList = errorList;
            this.StatementNodeList = statementNodeList;
        }

        public ChunkNode Root { get; }
        public ImmutableList<ParseError> ErrorList { get; }
        public List<Token> Tokens { get; }
        public List<StatementNode> StatementNodeList { get; }

        public static SyntaxTree Create(TextReader luaReader)
        {
            var parser = Parser.Parse(luaReader);
            return parser;
        }

        // For testing
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
