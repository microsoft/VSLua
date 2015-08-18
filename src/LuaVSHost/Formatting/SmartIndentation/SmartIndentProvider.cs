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
        [Import]
        private ISingletons singletons;

        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            return new SmartIndent(textView, this.singletons);
        }
    }
}
