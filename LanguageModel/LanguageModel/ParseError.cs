using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{
    public enum ErrorType
    {
        OutOfContextToken,
        IncompleteNode
    }
    public class ParseError
    {
        ParseError(ErrorType type, string message, int start, int end)
        {
            this.Type = type;
            this.Message = message;
            this.Start = start;
            this.End = end;
        }

        public ErrorType Type { get; private set; }
        public string Message { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
    }
}
