/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.SmartIndentation
{
    internal class SmartIndent : ISmartIndent
    {
        private ITextView textView;
        private ISingletons singletons;

        internal SmartIndent(ITextView textView, ISingletons singletons)
        {
            this.textView = textView;
            this.singletons = singletons;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            if (this.singletons.FormattingUserSettings.IndentStyle == vsIndentStyle.vsIndentStyleSmart)
            {
                ITextSnapshot snapshot = this.textView.TextSnapshot;
                int position = this.textView.Caret.Position.BufferPosition.Position;
                SourceText sourceText = this.singletons.SourceTextCache.Get(snapshot);

                int indentAmount = this.singletons.FeatureContainer.Formatter.SmartIndent(sourceText, position);

                return indentAmount;
            }

            return null;
        }
    }
}
