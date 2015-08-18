/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.SmartIndentation
{
    [Export(typeof(ISmartIndentProvider))]
    [ContentType(Constants.Language.Name)]
    [Name("LuaSmartIndentation")]
    internal class SmartIndentProvider : ISmartIndentProvider
    {
#pragma warning disable 0169, 0649
        [Import]
        private ISingletons singletons;
#pragma warning restore 0169, 0649

        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            return new SmartIndent(textView, this.singletons);
        }
    }
}
