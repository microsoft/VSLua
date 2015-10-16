using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // Represents a logical stack frame on the thread stack. 
    // Also implements the IDebugExpressionContext interface, which allows expression evaluation and watch windows.
    internal class AD7StackFrame : IDebugStackFrame2, IDebugExpressionContext2
    {
        private readonly AD7Engine m_engine;
        private readonly AD7Thread m_thread;

        private readonly uint m_line;
        
        private Variable[] m_locals;

        private string m_documentName;
        private string m_functionName;
        private uint m_lineNum;
        private bool m_hasSource;
        private uint m_numParameters;
        private int m_numLocals;

        public AD7StackFrame(AD7Engine engine, AD7Thread thread)
        {
            m_engine = engine;
            m_thread = thread;

            m_line = m_thread.Line;

            // TODO populate
            m_numLocals = thread.NumberOfLocals;

            // Now populate locals
            m_locals = new Variable[m_numLocals];
            // TODO: Get Locals here from thread
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the locals collection only.
        private void CreateLocalProperties(out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = (uint)m_numLocals;
            DEBUG_PROPERTY_INFO[] propInfo = new DEBUG_PROPERTY_INFO[m_thread.NumberOfLocals];

            for (int i = 0; i < propInfo.Length; i++)
            {
                AD7Property property = new AD7Property(m_thread.Locals[i]);
                propInfo[i] = property.ConstructDebugPropertyInfo(enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD);
            }

            enumObject = new AD7PropertyInfoEnum(propInfo);
        }

        public void SetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, int index, out FRAMEINFO frameInfo)
        {
            frameInfo = new FRAMEINFO();

            // The debugger is asking for the formatted name of the function which is displayed in the callstack window.
            // There are several optional parts to this name including the module, argument types and values, and line numbers.
            // The optional information is requested by setting flags in the dwFieldSpec parameter.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME) != 0)
            {
                string funcName = m_thread.StackFrames[index].GetFunc();

                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_MODULE) != 0)
                {
                    string rawSource = m_thread.StackFrames[index].GetSource();
                    var secs = rawSource.Split(' ');
                    string fileName = secs[secs.Length - 1];
                    fileName = fileName.Replace("\"", "");
                    funcName += " in " + Path.GetFileNameWithoutExtension(fileName);
                    frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_MODULE;
                }

                frameInfo.m_bstrFuncName = funcName;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME;

                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_LINES) != 0)
                {
                    frameInfo.m_bstrFuncName = funcName + " line " + m_thread.StackFrames[index].GetLine();
                    frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_LINES;
                }
            }

            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_LANGUAGE) != 0)
            {
                frameInfo.m_bstrLanguage = "Lua";
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_LANGUAGE;
            }

            // The debugger is requesting the IDebugStackFrame2 value for this frame info.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FRAME) != 0)
            {
                frameInfo.m_pFrame = this;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FRAME;
            }

            // Does this stack frame have symbols loaded?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO) != 0)
            {
                frameInfo.m_fHasDebugInfo = 1;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO;
            }

            // Is this frame stale?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_STALECODE) != 0)
            {
                frameInfo.m_fStaleCode = 0;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STALECODE;
            }
        }

        #region IDebugStackFrame2

        public int EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint pcelt, out IEnumDebugPropertyInfo2 ppEnum)
        {
            int hr;

            pcelt = 0;
            ppEnum = null;

            try
            {
                if (guidFilter == AD7Guids.guidFilterLocalsPlusArgs ||
                        guidFilter == AD7Guids.guidFilterAllLocalsPlusArgs ||
                        guidFilter == AD7Guids.guidFilterAllLocals)
                {
                    //CreateLocalsPlusArgsProperties(out elementsReturned, out enumObject);
                    CreateLocalProperties(out pcelt, out ppEnum);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == AD7Guids.guidFilterLocals)
                {
                    CreateLocalProperties(out pcelt, out ppEnum);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == AD7Guids.guidFilterArgs)
                {
                    //CreateParameterProperties(out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else
                {
                    hr = VSConstants.E_NOTIMPL;
                }
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }

            return hr;
        }

        // Gets the code context for this stack frame.The code context represents the current instruction pointer in this stack frame.
        int IDebugStackFrame2.GetCodeContext(out IDebugCodeContext2 memoryAddress)
        {
            memoryAddress = new AD7MemoryAddress(m_engine);
            return VSConstants.S_OK;
        }

        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            ppProperty = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetDocumentContext(out IDebugDocumentContext2 ppCxt)
        {
            // Assume all lines begin and end at the beginning of the line.
            TEXT_POSITION begTp = new TEXT_POSITION();
            begTp.dwColumn = 0;
            begTp.dwLine = m_line - 1;
            TEXT_POSITION endTp = new TEXT_POSITION();
            endTp.dwColumn = 0;
            endTp.dwLine = m_line -1;

            ppCxt = new AD7DocumentContext(m_thread.SourceFile, begTp, endTp);

            return VSConstants.S_OK;
        }

        public int GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
        {
            ppExprCxt = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetPhysicalStackRange(out ulong paddrMin, out ulong paddrMax)
        {
            paddrMax = ulong.MaxValue;
            paddrMin = ulong.MinValue;
            return VSConstants.E_NOTIMPL;
        }

        public int GetThread(out IDebugThread2 ppThread)
        {
            ppThread = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IDebugExpressionContext2

        public int GetName(out string pbstrName)
        {
            pbstrName = "";
            return VSConstants.E_NOTIMPL;
        }

        public int ParseText(string pszCode, enum_PARSEFLAGS dwFlags, uint nRadix, out IDebugExpression2 ppExpr, out string pbstrError, out uint pichError)
        {
            ppExpr = null;
            pbstrError = "";
            pichError = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}

