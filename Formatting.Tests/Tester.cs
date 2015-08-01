using System.Collections.Generic;
using LanguageService.Formatting;
using Xunit;
using System.IO;
using LanguageService;
using LanguageService;

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

        internal struct PasteInfo
        {
            internal string PasteString { get; }
            internal int From { get; }
            internal int To { get; }
            internal PasteInfo(string pasteString, int from, int to)
            {
                this.PasteString = pasteString;
                this.From = from;
                this.To = to;
            }
        }


        internal static string FormatOnPaste(string original, PasteInfo pasteInfo)
        {

            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();
            var buffer = host.CreateTextBuffer(original);
            //Manager.

            return null;
        }


        internal static string Format(string original)
        {
            LuaFeatureContainer featureContainer = new LuaFeatureContainer();
            List<TextEditInfo> textEdits = featureContainer.Formatter.Format(new SourceText(new StringReader(original)), null);

            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();
            var buffer = host.CreateTextBuffer(original);
            var edit = buffer.CreateEdit();

            foreach (var textEdit in textEdits)
            {
                edit.Replace(textEdit.Start, textEdit.Length, textEdit.ReplacingWith);
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
