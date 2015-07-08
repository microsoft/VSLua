using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal int TokenLeft { get; private set; }
        internal int TokenRight { get; private set; }

        internal RuleDescriptor(int tokenLeft, int tokenRight) // int because I don't know the type yet
        {
            this.TokenLeft = tokenLeft;
            this.TokenRight = tokenRight;
        }
    }
}
