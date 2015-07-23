using System.Collections.Immutable;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleDescriptor
    {
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

        internal ImmutableArray<TokenType> TokenRangeLeft { get; }
        internal ImmutableArray<TokenType> TokenRangeRight { get; }
    }
}
