using System.Collections.Generic;
using LanguageService.Formatting;
using Xunit;
using System.IO;
using LanguageModel;
using VSLua.Formatting;
using Microsoft.VisualStudio.Text;
using VSLua.Shared;

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
            internal int Start { get; }
            internal int Length { get; }
            internal PasteInfo(string pasteString, int start, int length)
            {
                this.PasteString = pasteString;
                this.Start = start;
                this.Length = length;
            }
        }


        internal static string FormatOnPaste(string original, PasteInfo pasteInfo)
        {
            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();

            var buffer = host.CreateTextBuffer(original);
            var prePastSnapshot = buffer.CurrentSnapshot;
            var bufferEdit = buffer.CreateEdit();
            bufferEdit.Replace(pasteInfo.Start, pasteInfo.Length, pasteInfo.PasteString);
            var bufferApplied = bufferEdit.Apply();
            SnapshotSpan? span = EditorUtilities.GetPasteSpan(prePastSnapshot, bufferApplied);

            if (span != null)
            {
                SnapshotSpan newSpan = (SnapshotSpan)span;
                SourceText sourceText = new SourceText(new StringReader(bufferApplied.GetText()));
                List<TextEditInfo> edits = Formatter.Format(sourceText, newSpan.Start.Position, newSpan.End.Position);
                var pastedBufferEdit = buffer.CreateEdit();
                foreach (TextEditInfo edit in edits)
                {
                    pastedBufferEdit.Replace(edit.Start, edit.Length, edit.ReplacingString);
                }
                var pasteApplied = pastedBufferEdit.Apply();
                return pasteApplied.GetText();
            }

            return original;
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
