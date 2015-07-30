using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleDescriptor
    {
        internal List<SyntaxKind> TokenRangeLeft { get; private set; }
        internal List<SyntaxKind> TokenRangeRight { get; private set; }

        internal RuleDescriptor(List<SyntaxKind> tokenLeft, List<SyntaxKind> tokenRight)
        {
            this.TokenRangeLeft = tokenLeft;
            this.TokenRangeRight = tokenRight;
        }

        internal RuleDescriptor(List<SyntaxKind> tokenLeft, SyntaxKind tokenRight) :
            this(tokenLeft, new List<SyntaxKind> { tokenRight })
        { }

        internal RuleDescriptor(SyntaxKind tokenLeft, List<SyntaxKind> tokenRight) :
            this(new List<SyntaxKind> { tokenLeft }, tokenRight)
        { }

        internal RuleDescriptor(SyntaxKind tokenLeft, SyntaxKind tokenRight) :
            this(new List<SyntaxKind> { tokenLeft }, new List<SyntaxKind> { tokenRight })
        { }

    }
}
