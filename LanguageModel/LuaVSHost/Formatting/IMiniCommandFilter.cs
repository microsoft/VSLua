using System;

using OLECommandFlags = Microsoft.VisualStudio.OLE.Interop.OLECMDF;

namespace VSLua.Formatting
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
        bool PreProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn);

        /// <summary>
        /// Operations that happen after the command has been processed
        /// </summary>
        void PostProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn, bool wasHandled);

        /// <summary>
        /// Puts the command status in commandStatus, a wrapper for the QueryStatus in IOleCommandTarget
        /// </summary>
        bool QueryCommandStatus(Guid guidCmdGroup, uint commandId, IntPtr commandText, out OLECommandFlags commandStatus);
    }
}
