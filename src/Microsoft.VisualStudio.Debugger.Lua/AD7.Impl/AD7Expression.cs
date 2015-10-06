using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // This class represents a succesfully parsed expression to the debugger. 
    // It is returned as a result of a successful call to IDebugExpressionContext2.ParseText
    // It allows the debugger to obtain the values of an expression in the debuggee. 
    // For the purposes of this sample, this means obtaining the values of locals and parameters from a stack frame.
    public class AD7Expression : IDebugExpression2 , IDebugExpressionEvaluator
    {
        private Variable m_var;

        public AD7Expression(Variable var)
        {
            m_var = var;
        }

        public int Abort()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int EvaluateAsync(enum_EVALFLAGS dwFlags, IDebugEventCallback2 pExprCallback)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int EvaluateSync(enum_EVALFLAGS dwFlags, uint dwTimeout, IDebugEventCallback2 pExprCallback, out IDebugProperty2 ppResult)
        {
            ppResult = new AD7Property(m_var);
            return VSConstants.S_OK;
        }

        public int GetMethodLocationProperty(string upstrFullyQualifiedMethodPlusOffset, IDebugSymbolProvider pSymbolProvider, IDebugAddress pAddress, IDebugBinder pBinder, out IDebugProperty2 ppProperty)
        {
            ppProperty = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetMethodProperty(IDebugSymbolProvider pSymbolProvider, IDebugAddress pAddress, IDebugBinder pBinder, int fIncludeHiddenLocals, out IDebugProperty2 ppProperty)
        {
            AD7Property method = new AD7Property(m_var);
            ppProperty = method;

            return VSConstants.S_OK;
        }

        public int Parse(string upstrExpression, enum_PARSEFLAGS dwFlags, uint nRadix, out string pbstrError, out uint pichError, out IDebugParsedExpression ppParsedExpression)
        {
            pbstrError = "";
            pichError = 0;
            ppParsedExpression = null;
            return VSConstants.E_NOTIMPL;
        }

        public int SetLocale(ushort wLangID)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetRegistryRoot(string ustrRegistryRoot)
        {
            return VSConstants.E_NOTIMPL;
        }        
    }
}