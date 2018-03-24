using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable SA1649 // File name must match first type name

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.GlobalName)]
    [Name(Constants.Colourization.GlobalName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    internal sealed class GlobalIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal GlobalIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.GlobalDisplayName;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.LocalName)]
    [Name(Constants.Colourization.LocalName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    internal sealed class LocalIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal LocalIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.LocalDisplayName;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.ParamName)]
    [Name(Constants.Colourization.ParamName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    internal sealed class ParamIdentifierClassificationFormat : ClassificationFormatDefinition
    {
        internal ParamIdentifierClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.ParamDisplayName;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Colourization.FieldName)]
    [Name(Constants.Colourization.FieldName)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    internal sealed class FieldClassificationFormat : ClassificationFormatDefinition
    {
        internal FieldClassificationFormat()
        {
            this.DisplayName = Constants.Colourization.FieldDisplayName;
            this.IsItalic = true;
        }
    }
}
