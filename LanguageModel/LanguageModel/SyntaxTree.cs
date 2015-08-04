using LanguageService.LanguageModel.TreeVisitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
            return new Parser().CreateSyntaxTree(luaStream);
        }

        public IEnumerable<SyntaxNodeOrToken> Next(SyntaxNodeOrToken syntaxNodeOrToken)
        {
            //TODO remove is-check once Immutable graph object bug is fixed. 
            if(syntaxNodeOrToken is SyntaxNode && ((SyntaxNode) syntaxNodeOrToken).Kind == SyntaxKind.ChunkNode)
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
            else
            {
                //TODO: do nothing here?
                //yield return syntaxNodeOrToken;
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
