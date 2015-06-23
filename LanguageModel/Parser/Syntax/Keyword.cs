using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Syntax
{
    internal class Keyword : SyntaxNode
    {
        private string value;

        Keyword(int start, int end, string value) : base(start, end)
        {
            this.value = value;
        }
    }
}
