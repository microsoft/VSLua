using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            parser.CreateParseTree(new MoveableStreamReader("C:/Users/t-kevimi/Documents/Visual Studio 2015/Projects/LanguageModel.cs/program.lua"));
            Console.Read();
        }
    }
}
