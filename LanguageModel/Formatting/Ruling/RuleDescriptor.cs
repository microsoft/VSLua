using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal TokenType[] TokenRangeLeft { get; private set; }
        internal TokenType[] TokenRangeRight { get; private set; }

        internal RuleDescriptor(TokenType[] tokenLeft, TokenType[] tokenRight)
        {
            this.TokenRangeLeft = tokenLeft;
            this.TokenRangeRight = tokenRight;
        }

        internal RuleDescriptor(TokenType[] tokenLeft, TokenType tokenRight) :
            this(tokenLeft, new TokenType[] { tokenRight })
        { }

        internal RuleDescriptor(TokenType tokenLeft, TokenType[] tokenRight) :
            this(new TokenType[] { tokenLeft }, tokenRight)
        { }

        internal RuleDescriptor(TokenType tokenLeft, TokenType tokenRight) :
            this(new TokenType[] { tokenLeft }, new TokenType[] { tokenRight })
        { }

    }
}
