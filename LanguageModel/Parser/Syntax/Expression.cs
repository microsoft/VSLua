using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Syntax
{
    abstract class Expression : SyntaxNode 
    {

        public bool isValidExpression() //TODO: implement
        {
            return true;
        }
    }
}
