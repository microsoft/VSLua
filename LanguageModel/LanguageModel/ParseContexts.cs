using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public enum Context
    {
        IfBlockContext,
        ProgramContext,
        ElseBlock,
        ElseIfBlock,
        ExpListContext,
        FuncBodyContext,
        NameListContext,
        DoStatementContext,
        WhileContext,
        RepeatStatementContext,
        ForStatementContext,
    }
}
