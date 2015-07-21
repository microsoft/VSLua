using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
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
            if(message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Type = type;
            this.Message = message;
            this.Start = start;
            this.End = end;
        }

        public ErrorType Type { get; }
        public string Message { get; }
        public int Start { get; }
        public int End { get; }
    }
}
