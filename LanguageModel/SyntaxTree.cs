using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public class SyntaxTree
    {
        public SyntaxTree(string fileName, SyntaxNode root, List<ParseError> errorList)
        {
            this.FileName = fileName;
            this.Root = root;
            this.ErrorList = errorList;
        }

        public string FileName { get; private set; }
        public SyntaxNode Root { get; private set; }
        public List<ParseError> ErrorList { get; private set; }
        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
            return null;
        }

        public void PrintSyntaxTree()
        {
            throw new NotImplementedException();
        }
    }
}
