using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate")]
    internal class LuaClassificationTypeDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.GlobalName)]
        internal ClassificationTypeDefinition Globals = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.LocalName)]
        internal ClassificationTypeDefinition Locals = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.ParamName)]
        internal ClassificationTypeDefinition Params = null;
    }
}
