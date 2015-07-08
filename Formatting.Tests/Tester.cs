using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageModel.Formatting;

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
    }
}
