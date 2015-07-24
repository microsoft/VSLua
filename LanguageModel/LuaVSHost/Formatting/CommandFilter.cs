using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting
{
    class CommandFilter : IOleCommandTarget
    {
        public CommandFilter()
        {
            this.MiniFilters = new List<IMiniCommandFilter>();
        }

        public IOleCommandTarget Next { get; set; }

        public IList<IMiniCommandFilter> MiniFilters { get; private set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            bool handled = false;
            int hresult = VSConstants.S_OK;

            // Give the commands a chance to pre-process the command
            foreach (IMiniCommandFilter filter in this.MiniFilters)
            {
                handled = filter.PreProcessCommand(pguidCmdGroup, nCmdID, pvaIn);

                if (handled)
                {
                    break;
                }

            }

            if (!handled)
            {
                hresult = this.Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            if (ErrorHandler.Succeeded(hresult))
            {
                // Let the command filters post-process any successful commands
                foreach (IMiniCommandFilter filter in this.MiniFilters)
                {
                    // Let all commands post-process for now.
                    filter.PostProcessCommand(pguidCmdGroup, nCmdID, pvaIn, handled);
                }
            }

            return hresult;
        }

        public void Close()
        {
            foreach (IMiniCommandFilter filter in this.MiniFilters)
            {
                filter.Close();
            }
            this.MiniFilters.Clear();
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint commandsCount, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            foreach (IMiniCommandFilter filter in this.MiniFilters)
            {
                OLECMDF commandStatus;
                if (filter.QueryCommandStatus(pguidCmdGroup, prgCmds[0].cmdID, pCmdText, out commandStatus))
                {
                    prgCmds[0].cmdf = (uint)commandStatus;
                    return VSConstants.S_OK;
                }
            }
            return this.Next.QueryStatus(pguidCmdGroup, commandsCount, prgCmds, pCmdText);
        }
    }
}
