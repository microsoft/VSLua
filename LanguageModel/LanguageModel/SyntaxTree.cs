using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public class SyntaxTree
    {
        public SyntaxTree(string fileName, ChunkNode root, List<ParseError> errorList)
        {
            this.FileName = fileName;
            this.Root = root;
            this.ErrorList = errorList;
        }

        //TODO: remove filename and finalize design based on host API
        public string FileName { get; private set; }
        public ChunkNode Root { get; private set; }
        public List<ParseError> ErrorList { get; private set; }
        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var treeStringWriter = new StringWriter();
            Root.ToString(treeStringWriter);
            return treeStringWriter.ToString();
        }
    }
}
