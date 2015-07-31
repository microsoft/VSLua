using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public enum ParsingContext
    {
        IfBlock,
        ChunkNodeBlock,
        ElseBlock,
        ElseIfBlock,
        DoStatementBlock,
        WhileBlock,
        RepeatStatementBlock,
        ForStatementBlock,
        FuncBodyBlock,
        ExpList,
        NameList,
        VarList,
        FieldList,
        FuncNameDotSeperatedNameList,
    }
}
