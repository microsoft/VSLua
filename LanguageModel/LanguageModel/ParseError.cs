using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace LanguageService
{
    public class ParseError
    {
        public ParseError(string message, int start, int end)
        {
            Requires.NotNull(message, nameof(message));

            this.Message = message;
            this.Start = start;
            this.End = end;
        }

        public string Message { get; }
        public int Start { get; }
        public int End { get; }

        public int Length
        {
            get { return this.End - this.Start; }
        }
    }
}
