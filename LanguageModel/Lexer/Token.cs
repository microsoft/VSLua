using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageModel
{
    public enum TokenType
    {
        EndKeyword,
        Identifier,
        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,
        OpenCurlyBrace,
        CloseCurlyBrace,
        Number,
        String,
        Unknown,
        EndOfFile,
        ThenKeyword,
        ElseKeyword,
        DoKeyword,
        RepeatKeyword,
        UntilKeyword,
        ElseIfKeyword,
        ModulusOperator,
        ExponentOperator,
        MinusOperator,
        TildeUnOp,
        LengthUnop,
        NotEqualsOperator,
        LessOrEqualOperator,
        GreaterOrEqualOperator,
        EqualityOperator,
        PlusOperator,
        MultiplyOperator,
        DivideOperator,
        FloorDivideOperator,
        BitwiseAndOperator,
        BitwiseRightOperator,
        BitwiseOrOperator,
        Dot,
        Comma,
        SemiColon,
        Colon,
        DoubleColon,
        AssignmentOperator,
        LessThanOperator,
        GreaterThanOperator,
        StringConcatOperator,
        BitwiseLeftOperator,
        AndBinop,
        BreakKeyword,
        FalseKeyValue,
        ForKeyword,
        FunctionKeyword,
        GotoKeyword,
        IfKeyword,
        InKeyword,
        LocalKeyword,
        NilKeyValue,
        NotUnop,
        OrBinop,
        WhileKeyword,
        TrueKeyValue,
        ReturnKeyword
    }

    public class Token
    {
        public int FullStart { get; private set; }
        public int Start { get; private set; }
        public string Text { get; private set; }
        public TokenType Type { get; private set; }
        public List<Trivia> LeadingTrivia { get; private set; } //TODO: change to Immutable List


        public Token(TokenType tokentype, string value, List<Trivia> trivia, int fullStart, int start)
        {
            this.Type = tokentype;
            this.Text = value;
            this.LeadingTrivia = trivia;
            this.FullStart = fullStart;
            this.Start = start;
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("==============================================\nTrivia:\n");

            foreach (Trivia triv in LeadingTrivia)
            {
                sb.Append("\t");
                sb.Append(triv.ToString());
                sb.Append("\n");
            }

            sb.Append("Data:\n\t");
            sb.Append(this.Type.ToString());
            sb.Append("\t");
            sb.Append(Text);
            sb.Append("\t");
            sb.Append(Start);

            return sb.ToString();
        }
    }
}
