using System.Runtime.CompilerServices;
using LanguageModel;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace VSLua.Shared
{
    internal static class SourceTextProvider
    {
        private static ConditionalWeakTable<ITextSnapshot, SourceText> sources =
            new ConditionalWeakTable<ITextSnapshot, SourceText>();

        internal static SourceText Get(ITextSnapshot textSnapshot)
        {
            Validate.IsNotNull(textSnapshot, nameof(textSnapshot));

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
