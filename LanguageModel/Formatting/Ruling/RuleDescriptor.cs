using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal List<TokenType> TokenRangeLeft { get; private set; }
        internal List<TokenType> TokenRangeRight { get; private set; }

        internal RuleDescriptor(List<TokenType> tokenLeft, List<TokenType> tokenRight)
        {
            this.TokenRangeLeft = tokenLeft;
            this.TokenRangeRight = tokenRight;
        }

        internal RuleDescriptor(List<TokenType> tokenLeft, TokenType tokenRight) :
            this(tokenLeft, new List<TokenType> { tokenRight })
        { }

        internal RuleDescriptor(TokenType tokenLeft, List<TokenType> tokenRight) :
            this(new List<TokenType> { tokenLeft }, tokenRight)
        { }

        internal RuleDescriptor(TokenType tokenLeft, TokenType tokenRight) :
            this(new List<TokenType> { tokenLeft }, new List<TokenType> { tokenRight })
        { }

    }
}
