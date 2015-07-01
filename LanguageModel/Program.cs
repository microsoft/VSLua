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
            Stream testProgramStream = File.OpenRead(@"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Input\if.lua");
            
            Parser parser = new Parser();
            parser.CreateNewParseTree(testProgramStream);
            
            
            //TestLexer();
            //Lexer.PrintTokens(testProgramStream);

            Console.Read();
        }

        static void TestLexer()
        {
            string path_input = @"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Input";
            string path_output = @"C:\Users\t-kevimi\Source\Repos\VSIDEProj.Lua\LanguageModel\Testing\Output";
            List<string> files_input = FileHelper.GetAllFileNamesInDirectory(path_input);
            List<string> files_output = FileHelper.GetAllFileNamesInDirectory(path_output);

            Tester.TestFidelity(path_input, files_input);
        }
    }
}
