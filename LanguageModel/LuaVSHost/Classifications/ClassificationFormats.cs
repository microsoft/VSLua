using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.GlobalName)]
    [Name("LuaGlobalClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GlobalIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal GlobalIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.GlobalDisplayName;

            this.ForegroundColor = (Color)ColorConverter.ConvertFromString("#EEEEEE");
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.LocalName)]
    [Name("LuaLocalClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LocalIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal LocalIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.LocalDisplayName;

            this.ForegroundColor = (Color)ColorConverter.ConvertFromString("#16D1AC");
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.ParamName)]
    [Name("LuaParamClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ParamIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal ParamIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.ParamDisplayName;

            this.ForegroundColor = (Color)ColorConverter.ConvertFromString("#99CCFF");
        }
    }
}
