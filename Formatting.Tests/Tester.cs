using System.Collections.Generic;
using LanguageService.Formatting;
using Xunit;

namespace Formatting.Tests
{
    internal static class Tester
    {
        internal static string Format(string original)
        {
            List<TextEditInfo> textEdits = Formatter.Format(original);

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
