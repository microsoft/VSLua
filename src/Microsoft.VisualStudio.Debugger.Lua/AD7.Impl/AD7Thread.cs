using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // This class implements IDebugThread2 which represents a thread running in a program.
    internal class AD7Thread : IDebugThread2, IDebugThread100
    {
        private readonly AD7Engine m_engine;

        public AD7Thread(AD7Engine engine)
        {
            m_engine = engine;
        }

        public int EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 ppEnum)
        {
            FRAMEINFO[] frameInfoArray = new FRAMEINFO[FrameCount];

            // Only top frame
            if(FrameCount == 1)
            {
                AD7StackFrame frame = new AD7StackFrame(m_engine, this);
                frame.SetFrameInfo(dwFieldSpec, 0, out frameInfoArray[0]);
            }
            else
            {
                for(int i =0; i<FrameCount;i++)
                {
                    AD7StackFrame frame = new AD7StackFrame(m_engine, this); // stackframe[]
                    frame.SetFrameInfo(dwFieldSpec, i, out frameInfoArray[i]);
                }
            }

            ppEnum = new AD7FrameInfoEnum(frameInfoArray);

            return VSConstants.S_OK;
        }

        public string SourceFile { get; set; }

        public string FuncName { get; set; }

        public uint Line { get; set; }

        public int NumberOfLocals { get; set; }

        public int FrameCount { get; set; }

        public List<Frame> StackFrames { get; set; }

        public List<Variable> Locals { get; set; }

        public int GetName(out string pbstrName)
        {
            // Dummy value, will only ever be main thread
            pbstrName = "Lua Main Thread";
            return VSConstants.S_OK;
        }

        public int SetThreadName(string pszName)
        {
            return VSConstants.S_OK;
        }

        public int GetProgram(out IDebugProgram2 ppProgram)
        {
            ppProgram = this.m_engine;
            return VSConstants.S_OK;
        }

        public int CanSetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            return VSConstants.S_FALSE;
        }

        public int SetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetThreadId(out uint pdwThreadId)
        {
            // Dummy value
            pdwThreadId = 1234;
            return VSConstants.S_OK;
        }

        public int Suspend(out uint pdwSuspendCount)
        {
            pdwSuspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int Resume(out uint pdwSuspendCount)
        {
            pdwSuspendCount = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetThreadProperties(enum_THREADPROPERTY_FIELDS dwFields, THREADPROPERTIES[] ptp)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetLogicalThread(IDebugStackFrame2 pStackFrame, out IDebugLogicalThread2 ppLogicalThread)
        {
            ppLogicalThread = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetThreadProperties100(uint dwFields, THREADPROPERTIES100[] ptp)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetFlags(out uint pFlags)
        {
            pFlags = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int SetFlags(uint flags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int CanDoFuncEval()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetThreadDisplayName(out string bstrDisplayName)
        {
            bstrDisplayName = "";
            return VSConstants.E_NOTIMPL;
        }

        public int SetThreadDisplayName(string bstrDisplayName)
        {
            return VSConstants.E_NOTIMPL;
        }
    }
}
