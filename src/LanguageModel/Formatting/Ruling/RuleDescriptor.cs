using System.Collections.Immutable;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal RuleDescriptor(ImmutableArray<SyntaxKind> tokenLeft, ImmutableArray<SyntaxKind> tokenRight)
        {
            this.TokenRangeLeft = tokenLeft;
            this.TokenRangeRight = tokenRight;
        }

        internal RuleDescriptor(ImmutableArray<SyntaxKind> tokenLeft, SyntaxKind tokenRight) :
            this(tokenLeft, ImmutableArray.Create(tokenRight))
        { }

        internal RuleDescriptor(SyntaxKind tokenLeft, ImmutableArray<SyntaxKind> tokenRight) :
            this(ImmutableArray.Create(tokenLeft), tokenRight)
        { }

        internal RuleDescriptor(SyntaxKind tokenLeft, SyntaxKind tokenRight) :
            this(ImmutableArray.Create(tokenLeft), ImmutableArray.Create(tokenRight))
        { }

        internal ImmutableArray<SyntaxKind> TokenRangeLeft { get; }
        internal ImmutableArray<SyntaxKind> TokenRangeRight { get; }
    }
}
