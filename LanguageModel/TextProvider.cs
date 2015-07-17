using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService;
using System.Runtime.CompilerServices;
using System.IO;

namespace LanguageModel
{
    public static class TextProvider
    {
        private static List<Token> parseTree = null;
        
        public static void Update(TextReader textReader)
        {
            TextProvider.parseTree = Lexer.Tokenize(textReader);
        }
    }
}
