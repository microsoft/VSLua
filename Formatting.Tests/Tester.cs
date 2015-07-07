using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formatting.Tests
{
    internal static class Tester
    {
        internal static string Format(string original)
        {

            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();

            var buffer = host.CreateTextBuffer(original);
            var edit = buffer.CreateEdit();
            var applied = edit.Apply();

            return applied.GetText();
        }
    }
}
