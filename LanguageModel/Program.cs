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
			Lexer lexer = new Lexer();
			string path_input = "C:\\Users\\t-lufern\\Desktop\\LanguageModel.cs\\LanguageModel.cs\\Testing\\Input";
			string path_output = "C:\\Users\\t-lufern\\Desktop\\LanguageModel.cs\\LanguageModel.cs\\Testing\\Output";
			List<string> files_input = FileHelper.GetAllFileNamesInDirectory(path_input);
			List<string> files_output = FileHelper.GetAllFileNamesInDirectory(path_output);

			Tester.TestFidelity(path_input, files_input, lexer);

			Tester.TestTokens(path_input, path_output, lexer);

			Console.Read();


        }
    }
}
