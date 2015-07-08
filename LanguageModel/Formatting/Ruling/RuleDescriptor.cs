using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal TokenType[] TokenLeft { get; private set; }
        internal TokenType[] TokenRight { get; private set; }

        internal RuleDescriptor(TokenType[] tokenLeft, TokenType[] tokenRight)
        {
            this.TokenLeft = tokenLeft;
            this.TokenRight = tokenRight;
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
