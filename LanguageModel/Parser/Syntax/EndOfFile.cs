using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Syntax
{
    internal class EndOfFileNode 
    {
        private int programLength;

        EndOfFileNode(int length)
        {
            this.programLength = length;
        }
    }
}
