using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // And implementation of IDebugCodeContext2 and IDebugMemoryContext2. 
    // IDebugMemoryContext2 represents a position in the address space of the machine running the program being debugged.
    // IDebugCodeContext2 represents the starting position of a code instruction. 
    // For most run-time architectures today, a code context can be thought of as an address in a program's execution stream.
    internal class AD7MemoryAddress : IDebugCodeContext2, IDebugCodeContext100
    {
        private readonly AD7Engine m_engine;

        private IDebugDocumentContext2 m_documentContext;

        public AD7MemoryAddress(AD7Engine engine)
        {
            m_engine = engine;
        }

        public void SetDocumentContext(IDebugDocumentContext2 docContext)
        {
            m_documentContext = docContext;
        }

        #region IDebugCodeContext2

        public int GetName(out string pbstrName)
        {
            pbstrName = "";
            return VSConstants.E_NOTIMPL;
        }

        public int GetInfo(enum_CONTEXT_INFO_FIELDS dwFields, CONTEXT_INFO[] pinfo)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Add(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            ppMemCxt = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Subtract(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            ppMemCxt = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Compare(enum_CONTEXT_COMPARE Compare, IDebugMemoryContext2[] rgpMemoryContextSet, uint dwMemoryContextSetLen, out uint pdwMemoryContext)
        {
            pdwMemoryContext = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetDocumentContext(out IDebugDocumentContext2 ppSrcCxt)
        {
            ppSrcCxt = m_documentContext;
            return VSConstants.S_OK;
        }

        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            pbstrLanguage = "Lua";
            pguidLanguage = new Guid("88A1F488-9D00-4896-A255-6F8251208B90"); // TODO
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugCodeContext100

        public int GetProgram(out IDebugProgram2 ppProgram)
        {
            ppProgram = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion        
    }
}
