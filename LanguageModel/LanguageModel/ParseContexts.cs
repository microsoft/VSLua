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
