using LanguageModel;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VSLua.Shared
{
    internal static class SourceTextProvider
    {
        private static ConditionalWeakTable<ITextSnapshot, SourceText> sources =
            new ConditionalWeakTable<ITextSnapshot, SourceText>();

        internal static SourceText Get(ITextSnapshot textSnapshot)
        {
            if (textSnapshot == null)
            {
                throw new ArgumentNullException("textSnapshot");
            }

            SourceText sourceText = null;
            if (sources.TryGetValue(textSnapshot, out sourceText))
            {
                return sourceText;
            }
    
            sourceText = new SourceText(new TextSnapshotToTextReader(textSnapshot));
            sources.Add(textSnapshot, sourceText);
            return sourceText;
        }

    }
}
