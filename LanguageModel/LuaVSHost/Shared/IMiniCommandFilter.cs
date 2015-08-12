using System;

using OLECommandFlags = Microsoft.VisualStudio.OLE.Interop.OLECMDF;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal interface IMiniCommandFilter
    {
        /// <summary>
        /// Operations that happen at the close of whatever the IMiniCommandFilter is attached to.
        /// </summary>
        void Close();

        /// <summary>
        /// Operations that happen before the command has been processed
        /// </summary>
        /// <param name="guidCmdGroup">The Guid Cmd Group for the command</param>
        /// <param name="commandId">The command ID</param>
        /// <param name="variantIn">The variant for the command</param>
        /// <returns>
        /// Return true if the command proceeded in affecting the editor environment, i.e. if it formatted
        /// the document, or a selection, otherwise returns false if the editor environment
        /// did not change.
        /// </returns>
        bool PreProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn);

        /// <summary>
        /// Operations that happen after the command has been processed
        /// </summary>
        /// <param name="guidCmdGroup">The Guid Cmd Group for the command</param>
        /// <param name="commandId">The command ID</param>
        /// <param name="variantIn">The variant for the command</param>
        /// <param name="wasHandled">Whether or not the pre-process command was handled</param>
        void PostProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn, bool wasHandled);

        /// <summary>
        /// Puts the command status in commandStatus, a wrapper for the QueryStatus in IOleCommandTarget
        /// </summary>
        /// <param name="guidCmdGroup">The Guid Cmd Group for the command</param>
        /// <param name="commandId">The command ID</param>
        /// <param name="commandText">The command text</param>
        /// <param name="commandStatus">The command status</param>
        /// <returns>Returns true if the query succeeded otherwise false</returns>
        bool QueryCommandStatus(Guid guidCmdGroup, uint commandId, IntPtr commandText, out OLECommandFlags commandStatus);
    }
}
