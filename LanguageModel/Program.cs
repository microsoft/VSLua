using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream testProgramStream = File.OpenRead(@"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel.Tests\CorrectSampleLuaFiles\if.lua");
            IEnumerable<Token> tokenEnum =  Lexer.Tokenize(testProgramStream);

            foreach(Token tok in tokenEnum)
            {
                Console.WriteLine(tok.ToString());
            }
            Console.Read();
        }
    }
}
