using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting
{
    internal static class TokenRange
    {
        internal static List<TokenType> Any = TokenRange.Fill(Enum.GetValues(typeof(TokenType)));

        private static List<TokenType> Fill(Array values)
        {

            List<TokenType> tokenTypes = new List<TokenType>();

            foreach (TokenType tokenType in values)
            {
                tokenTypes.Add(tokenType);
            }

            return tokenTypes;
        }

    }
}
