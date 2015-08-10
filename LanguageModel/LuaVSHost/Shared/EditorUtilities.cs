using System;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal static class EditorUtilities
    {
        internal static SnapshotSpan CreateSnapshotSpan(ITextSnapshot snapshot, int position, int length)
        {
            // Assume a bogus (negative) position to be at the end.
            if (position < 0)
            {
                position = snapshot.Length;
            }

            position = Math.Min(position, snapshot.Length);
            length = Math.Max(0, Math.Min(length, snapshot.Length - position));

            return new SnapshotSpan(snapshot, position, length);
        }

        internal static SnapshotSpan? GetPasteSpan(ITextSnapshot prePasteSnapshot, ITextSnapshot postPasteSnapshot)
        {
            INormalizedTextChangeCollection changes = prePasteSnapshot.Version.Changes;
            if (changes != null && changes.Count > 0)
            {
                ITextChange firstChange = changes[0];
                ITextChange lastChange = changes[changes.Count - 1];

                SnapshotSpan newSpan = new SnapshotSpan(postPasteSnapshot, Span.FromBounds(firstChange.NewPosition, lastChange.NewEnd));

                return newSpan;
            }

            return null;
        }
    }
}
