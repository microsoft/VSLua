using System.Runtime.CompilerServices;
using LanguageModel;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    /// <summary>
    /// This class will be changed to non-static once I merge this branch with the branch that has the Core class that
    /// holds everything.
    /// </summary>
    internal static class SourceTextProvider
    {
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

        private static ConditionalWeakTable<ITextSnapshot, SourceText> sources =
            new ConditionalWeakTable<ITextSnapshot, SourceText>();
    }
}
