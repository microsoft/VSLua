using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal sealed class ParsedToken
    {

        internal Token Token { get; private set; }
        internal int BlockLevel { get; private set; }
        internal SyntaxNode Node { get; private set; }

        internal ParsedToken(Token token, int blockLevel, SyntaxNode node)
        {
            this.Token = token;
            this.BlockLevel = blockLevel;
            this.Node = node;
        }

    }
}
