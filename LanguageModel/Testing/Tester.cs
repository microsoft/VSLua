using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageModel
{

	enum Type
	{
		TRIVIA, TOKEN,
	}

	internal sealed class Tester
	{
		private static string BuildTriviaString(Token token)
		{

			string trivia_string = "";
			foreach (Trivia trivia in token.LeadingTrivia)
			{
				trivia_string += trivia.trivia;
			}

			return trivia_string;
		}

		private static string GetTokenFromString(int start, int length, ref string fileString)
		{
			return fileString.Substring(start, length);
		}

		private static string GetTriviaFromString(int fullStart, int start, ref string fileString)
		{
			return fileString.Substring(fullStart, start - fullStart);
		}

		private static string GetTriviaTokenFromString(int fullStart, int start, int length, ref string fileString)
		{
			return GetTriviaFromString(fullStart, start, ref fileString) + GetTokenFromString(start, length, ref fileString);
        }

		private static void DisplayInconsistency(string expected, string output, Token token, Type type, string file) // 0 for trivia, 1 for token
		{
			Console.WriteLine("In " + file + " on index: " + token.FullStart + " to " + (token.Start + token.Text.Length));
			Console.WriteLine("For " + type.ToString() + " expected: \"" + ConvertStringToSymbols(expected) + "\", but got: \"" + ConvertStringToSymbols(output) + "\".");
			Console.WriteLine();
		}

		private static string ConvertStringToSymbols(string s)
		{
			string new_string = "";

			foreach (char c in s)
			{
				if (c == '\r')
				{
					new_string += "\\r";
				}
				else if (c == '\n')
				{
					new_string += "\\n";
				}
				else if (c == '\t')
				{
					new_string += "\\t";
				}
				else if (c == ' ')
				{
					new_string += "\\s";
				}
				else
				{
					new_string += c;
				}
			}

			return new_string;
		}

		private static bool CompareToken(Token token, ref string fileString, string file)
		{

			string token_string = GetTokenFromString(token.Start, token.Text.Length, ref fileString);
			string trivia_string = GetTriviaFromString(token.FullStart, token.Start, ref fileString);

			bool passed = true;

			if (token_string != token.Text)
			{
				DisplayInconsistency(token_string, token.Text, token, Type.TOKEN, file);
				passed = false;
			}

			string trivia_from_token = BuildTriviaString(token);
			if (trivia_string != trivia_from_token)
			{
				DisplayInconsistency(trivia_string, trivia_from_token, token, Type.TRIVIA, file);
				passed = false;
			}

			return passed;
		}

		public static void TestFidelity(string path_input, List<string> files_input, Lexer lexer)
		{
			foreach (string file in files_input)
			{
				string file_location = path_input + "\\" + file;
				Stream lua_test = File.OpenRead(file_location);
                char p = lua_test.Peek();
                string fileString = (p == 'ï' ? "ï»¿" : "") + File.ReadAllText(file_location); // TODO: Deal with BOM
                //string fileString = File.ReadAllText(file_location); // TODO: Deal with BOM

                List<Token> tokens = lexer.Tokenize(lua_test);

				bool file_pass = true;



				foreach (Token token in tokens)
				{
					if (!CompareToken(token, ref fileString, file))
					{
						file_pass = false;
					}
				}

				if (file_pass)
				{
					Console.WriteLine(file + " passed the fidelity test!");
				}
				else
				{
					Console.WriteLine(file + " failed the fidelity test!");
				}
			}
		}

		public static void TestTokens(string path_input, string path_output, Lexer lexer)
		{
			List<string> input_files = FileHelper.GetAllFileNamesInDirectory(path_input);
			List<string> output_files = FileHelper.GetAllFileNamesInDirectory(path_output);

			List<string> test_files = input_files.Where(x => output_files.Contains(x.Substring(0, x.IndexOf('.')) + ".out")).ToList();

			foreach (string file in test_files)
			{

				string outfile_name = file.Substring(0, file.IndexOf('.')) + ".out";

				Stream lua_stream = File.OpenRead(path_input + "\\" + file);
				
				List<Token> tokens = lexer.Tokenize(lua_stream);

				string token_strings = "";

				foreach (Token token in tokens)
				{
					token_strings += token.Type + "\n";
				}

				string expected_output = File.ReadAllText(path_output + "\\" + outfile_name);

				if (expected_output == token_strings)
				{
					Console.WriteLine(file + " passed the token test!");
				}else
				{
					Console.WriteLine("In " + file + " expected these tokens: ");
					Console.Write(expected_output);
					Console.WriteLine("But got: ");
					Console.Write(token_strings);
					Console.WriteLine("\n");
				}

			}
			

		}

	}
}
