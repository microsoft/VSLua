using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LanguageModel;

namespace LanguageModel
{
    public class LexerTests
    {

        [Fact]
        public void TestConcat()
        {
            Stream testProgramStream = File.OpenRead(@"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel.Tests\CorrectSampleLuaFiles\if.lua");
            IEnumerable<Token> tokenEnum = Lexer.Tokenize(testProgramStream);

            foreach (Token tok in tokenEnum)
            {
                Console.WriteLine(tok.ToString());
            }
            Console.Read();
        }

        private string concat(string a, string b)
        {
            return a + b;
        }



    }
}
