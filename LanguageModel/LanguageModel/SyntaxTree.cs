using LanguageService.LanguageModel.TreeVisitors;
using Newtonsoft.Json;
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
            Stream luaStream = File.OpenRead(filename);
            return new Parser().CreateSyntaxTree(luaStream);
        }

        public IEnumerable<SyntaxNodeOrToken> Next(SyntaxNodeOrToken syntaxNodeOrToken)
        {
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

        private bool IsLeafNode(SyntaxNodeOrToken node)
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
