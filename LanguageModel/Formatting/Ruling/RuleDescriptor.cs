using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal readonly ImmutableArray<TokenType> TokenRangeLeft;
        internal readonly ImmutableArray<TokenType> TokenRangeRight;

        internal RuleDescriptor(ImmutableArray<TokenType> tokenLeft, ImmutableArray<TokenType> tokenRight)
        {
            this.TokenRangeLeft = tokenLeft;
            this.TokenRangeRight = tokenRight;
        }

        internal RuleDescriptor(ImmutableArray<TokenType> tokenLeft, TokenType tokenRight) :
            this(tokenLeft, new ImmutableArray<TokenType> { tokenRight })
        { }

        internal RuleDescriptor(TokenType tokenLeft, ImmutableArray<TokenType> tokenRight) :
            this(new ImmutableArray<TokenType> { tokenLeft }, tokenRight)
        { }

        internal RuleDescriptor(TokenType tokenLeft, TokenType tokenRight) :
            this(new ImmutableArray<TokenType> { tokenLeft }, new ImmutableArray<TokenType> { tokenRight })
        { }

    }
}
