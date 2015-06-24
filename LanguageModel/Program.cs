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
            //Parser parser = new Parser();
            //parser.CreateParseTree(File.OpenRead(@"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Input\test.lua"));

            TestLexer();

			Console.Read();
        }

        static void TestLexer()
        {
            Lexer lexer = new Lexer();
            string path_input = @"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Input";
            string path_output = @"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Output";
            List<string> files_input = FileHelper.GetAllFileNamesInDirectory(path_input);
            List<string> files_output = FileHelper.GetAllFileNamesInDirectory(path_output);

            Tester.TestFidelity(path_input, files_input, lexer);
        }
    }
}
