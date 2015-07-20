using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLua.Shared
{
    internal static class EditorUtilities
    {
        internal static SnapshotSpan CreateSnapshotSpan(ITextSnapshot snapshot, int position, int length)
        {
            position = Math.Min(position, snapshot.Length);
            length = Math.Max(0, Math.Min(length, snapshot.Length - position));
            return new SnapshotSpan(snapshot, position, length);
        }
    }
}
