using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace LanguageModel
{
	public class FileHelper
	{
		public static List<string> GetAllFileNamesInDirectory(string path)
		{
			List<string> fileNames = new List<string>();

			foreach (string s in Directory.GetFiles(path).Select(Path.GetFileName)){
				fileNames.Add(s);
			}

			return fileNames;
		}
	}
}
