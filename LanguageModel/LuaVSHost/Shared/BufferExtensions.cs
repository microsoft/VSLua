using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Projection;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal static class BufferExtensions
    {
        internal static string GetFilePath(this ITextBuffer textBuffer)
        {
            string filePath = GetFilePathFromTextDocument(textBuffer);

            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath;
            }

            IProjectionBuffer projectionBuffer = textBuffer as IProjectionBuffer;

            if (projectionBuffer == null)
            {
                return null;
            }

            List<string> filePaths = new List<string>();

            foreach (ITextBuffer buffer in projectionBuffer.SourceBuffers)
            {
                filePath = GetFilePathFromTextDocument(buffer);

                if (!string.IsNullOrEmpty(filePath))
                {
                    filePaths.Add(filePath);
                }
            }

            Debug.Assert(filePaths.Count <= 1, "Why is there more than one buffer with a file path?");

            if (filePaths.Count > 0)
            {
                return filePaths[0];
            }

            return null;
        }

        private static string GetFilePathFromTextDocument(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
            {
                return null;
            }

            ITextDocument document;

            if (textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document) && document != null && document.TextBuffer != null)
            {
                return document.FilePath;
            }

            return null;
        }
    }
}
