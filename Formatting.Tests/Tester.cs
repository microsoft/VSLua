using System.Collections.Generic;
using LanguageService.Formatting;
using Xunit;
using System.IO;
using LanguageModel;

namespace Formatting.Tests
{
    internal static class Tester
    {
        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        internal static string Format(string original)
        {
            List<TextEditInfo> textEdits = Formatter.Format(new SourceText(new StringReader(original)), 0, original.Length);

            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();

            var buffer = host.CreateTextBuffer(original);
            var edit = buffer.CreateEdit();

            foreach (var textEdit in textEdits)
            {
                edit.Replace(textEdit.Start, textEdit.Length, textEdit.ReplacingString);
            }

            var applied = edit.Apply();

            return applied.GetText();
        }

        internal static void GeneralTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        internal static void GeneralTest(string original, string expected1, string expected2)
        {
            string actual1 = Tester.Format(original);
            //string actual2 = Tester.Format(original);
            Assert.Equal(expected1, actual1);
            //Assert.Equal(expected2, actual2);
        }
    }
}
