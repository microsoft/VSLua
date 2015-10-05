using System;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // This class manages breakpoints for the engine. 
    internal class BreakpointManager
    {
        private AD7Engine m_engine;
        private List<AD7PendingBreakpoint> m_pendingBreakpoints;
        private Dictionary<string, AD7BoundBreakpoint> m_boundBreakpoints;

        public BreakpointManager(AD7Engine engine)
        {
            m_engine = engine;
            m_pendingBreakpoints = new List<AD7PendingBreakpoint>();
            m_boundBreakpoints = new Dictionary<string, AD7BoundBreakpoint>();
        }

        // A helper method used to construct a new pending breakpoint.
        // Called when new breakpoint set
        public void CreatePendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, out IDebugPendingBreakpoint2 ppPendingBP)
        {
            AD7PendingBreakpoint pendingBreakpoint = new AD7PendingBreakpoint(pBPRequest, m_engine, this);
            ppPendingBP = (IDebugPendingBreakpoint2)pendingBreakpoint;
            m_pendingBreakpoints.Add(pendingBreakpoint);
        }

        public void StoreBoundBreakpoint(string fileandline, AD7BoundBreakpoint bp)
        {
            // store bound breakpoint in list, need to be able to get
            m_boundBreakpoints.Add(fileandline, bp);
        }

        // Called from the engine's detach method to remove the debugger's breakpoint instructions.
        public void ClearBoundBreakpoints()
        {
        }

        internal AD7BoundBreakpoint GetBoundBreakpoint(string fileandline)
        {
            if(m_boundBreakpoints.ContainsKey(fileandline))
            {
                return m_boundBreakpoints[fileandline];
            }

            throw new InvalidOperationException();
        }
    }
}
