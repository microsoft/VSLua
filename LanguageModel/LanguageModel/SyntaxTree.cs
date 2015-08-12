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

        public static SyntaxTree Create(TextReader luaReader)
        {
            return new Parser().CreateSyntaxTree(luaReader);
        }

        // For testing
        public static SyntaxTree Create(string filename)
        {
            TextReader luaReader = File.OpenText(filename);
            return new Parser().CreateSyntaxTree(luaReader);
        }

        public static SyntaxTree CreateFromString(string program)
        {
            TextReader luaStream = new StringReader(program);
            return new Parser().CreateSyntaxTree(luaStream);
        }

        public IEnumerable<SyntaxNodeOrToken> Next(SyntaxNodeOrToken syntaxNodeOrToken)
        {
            if (syntaxNodeOrToken is SyntaxNode && ((SyntaxNode)syntaxNodeOrToken).Kind == SyntaxKind.ChunkNode)
            {
                yield return syntaxNodeOrToken;
            }

            if (!IsLeafNode(syntaxNodeOrToken))
            {
                foreach (var node in ((SyntaxNode)syntaxNodeOrToken).Children)
                {
                    yield return node;

                    foreach (var nextNode in Next(node))
                    {
                        yield return nextNode;
                    }
                }
            }
        }

        public static bool IsLeafNode(SyntaxNodeOrToken node)
        {
            if (node is Token)
            {
                return true;
            }
            else
            {
                return (node as SyntaxNode).Children.Count == 0;
            }
        }

        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
        }
    }
}
