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
        public SyntaxTree(string fileName, ChunkNode root, ImmutableList<ParseError> errorList)
        {
            this.FileName = fileName;
            this.Root = root;
            this.ErrorList = errorList;
        }

        //TODO: remove filename and finalize design based on host API
        public string FileName { get; }
        public ChunkNode Root { get; }
        public ImmutableList<ParseError> ErrorList { get; }
        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var visitor = new ToStringVisitor();
            Root.Accept(visitor);
            return visitor.indentingWriter.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(Root);
        }
    }
}
