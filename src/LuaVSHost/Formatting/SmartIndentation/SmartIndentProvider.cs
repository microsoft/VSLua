using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [Import]
        private ISingletons singletons = null;

        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            return new SmartIndent(textView, this.singletons);
        }
    }
}
