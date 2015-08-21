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
        [Name(Constants.Colourization.GlobalName), Export]
        internal ClassificationTypeDefinition Globals { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.LocalName), Export]
        internal ClassificationTypeDefinition Locals { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.ParamName), Export]
        internal ClassificationTypeDefinition LuaParams { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Colourization.FieldName), Export]
        internal ClassificationTypeDefinition Fields { get; set; }
    }
}
