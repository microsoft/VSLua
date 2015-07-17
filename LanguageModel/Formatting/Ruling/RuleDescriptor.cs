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
            this(tokenLeft, ImmutableArray.Create(tokenRight))
        { }

        internal RuleDescriptor(TokenType tokenLeft, ImmutableArray<TokenType> tokenRight) :
            this(ImmutableArray.Create(tokenLeft), tokenRight)
        { }

        internal RuleDescriptor(TokenType tokenLeft, TokenType tokenRight) :
            this(ImmutableArray.Create(tokenLeft), ImmutableArray.Create(tokenRight))
        { }

    }
}
