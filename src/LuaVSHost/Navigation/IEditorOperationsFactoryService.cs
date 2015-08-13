using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Navigation
{
    /// <summary>
    /// A service that provides <see cref="IEditorOperations"/> objects.
    /// </summary>
    /// <remarks>This is a MEF component part, and should be imported as follows:
    /// [Import]
    /// IEditorOperationsFactoryService factory = null;
    /// </remarks>
    public interface IEditorOperationsFactoryService
    {
        /// <summary>
        /// Gets the <see cref="IEditorOperations"/> objects for the specified <see cref="ITextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="ITextView"/>.</param>
        /// <returns>The <see cref="IEditorOperations"/>. </returns>
        IEditorOperations GetEditorOperations(ITextView textView);
    }
}
