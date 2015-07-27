using System.Runtime.CompilerServices;
using LanguageModel;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal static class SourceTextProvider
    {
        private static ConditionalWeakTable<ITextSnapshot, SourceText> sources =
            new ConditionalWeakTable<ITextSnapshot, SourceText>();

        internal static SourceText Get(ITextSnapshot textSnapshot)
        {
            Validation.Requires.NotNull(textSnapshot, nameof(textSnapshot));

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
