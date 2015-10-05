using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Lua;
using System.IO;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // This class represents a pending breakpoint which is an abstract representation of a breakpoint before it is bound.
    // When a user creates a new breakpoint, the pending breakpoint is created and is later bound. The bound breakpoints
    // become children of the pending breakpoint.
    internal class AD7PendingBreakpoint : IDebugPendingBreakpoint2
    {
        private IDebugBreakpointRequest2 m_pBPRequest;
        private BP_REQUEST_INFO m_bpRequestInfo;
        private AD7Engine m_engine;
        private BreakpointManager m_bpManager;

        private System.Collections.Generic.List<AD7BoundBreakpoint> m_boundBreakpoints;

        private bool m_enabled;
        private bool m_deleted;

        public AD7PendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, AD7Engine engine, BreakpointManager bpManager)
        {
            m_pBPRequest = pBPRequest;
            BP_REQUEST_INFO[] requestInfo = new BP_REQUEST_INFO[1];
            EngineUtils.CheckOk(m_pBPRequest.GetRequestInfo(enum_BPREQI_FIELDS.BPREQI_BPLOCATION, requestInfo));
            m_bpRequestInfo = requestInfo[0];

            m_engine = engine;
            m_bpManager = bpManager;
            m_boundBreakpoints = new System.Collections.Generic.List<AD7BoundBreakpoint>();

            m_enabled = true;
            m_deleted = false;
        }

        public int Bind()
        {
            IDebugDocumentPosition2 docPosition = (IDebugDocumentPosition2)(Marshal.GetObjectForIUnknown(m_bpRequestInfo.bpLocation.unionmember2));

            string filename;
            docPosition.GetFileName(out filename);

            TEXT_POSITION[] startPosition = new TEXT_POSITION[1];
            TEXT_POSITION[] endPosition = new TEXT_POSITION[1];
            EngineUtils.CheckOk(docPosition.GetRange(startPosition, endPosition));

            Command bpCommand = new BreakpointCommand(Path.GetFileName(filename), (int)startPosition[0].dwLine + 1);
            m_engine.EnqueueCommand(bpCommand);

            AD7DocumentContext docContext = new AD7DocumentContext(filename, startPosition[0], endPosition[0]);

            AD7BreakpointResolution breakpointResolution = new AD7BreakpointResolution(this.m_engine, docContext);
            AD7BoundBreakpoint boundBreakpoint = new AD7BoundBreakpoint(this.m_engine, this, breakpointResolution);

            string fileandline = Path.GetFileName(filename) + ((int)startPosition[0].dwLine + 1).ToString();
            m_bpManager.StoreBoundBreakpoint(fileandline, boundBreakpoint);

            return VSConstants.S_OK;
        }

        public int CanBind(out IEnumDebugErrorBreakpoints2 ppErrorEnum)
        {
            ppErrorEnum = null;
            return VSConstants.S_OK;
        }

        public int Delete()
        {
            // TODO: Actually delete
            return VSConstants.S_OK;
        }

        public int Enable(int fEnable)
        {
            // TODO: Support (dis)enable
            return VSConstants.S_OK;
        }

        public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        public int EnumErrorBreakpoints(enum_BP_ERROR_TYPE bpErrorType, out IEnumDebugErrorBreakpoints2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetBreakpointRequest(out IDebugBreakpointRequest2 ppBPRequest)
        {
            ppBPRequest = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetState(PENDING_BP_STATE_INFO[] pState)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetCondition(BP_CONDITION bpCondition)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Virtualize(int fVirtualize)
        {
            return VSConstants.S_OK;
        }        
    }
}
