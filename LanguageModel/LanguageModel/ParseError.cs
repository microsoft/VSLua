using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService
{
    public class ParseError
    {
        public ParseError(string message, int start, int end)
        {
            if(message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Message = message;
            this.Start = start;
            this.End = end;
        }

        public string Message { get; }
        public int Start { get; }
        public int End { get; }
    }
}
