using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService.Formatting;

namespace LanguageService
{
    public sealed class LuaFeatureContainer
    {
        internal ParseTreeCache ParseTreeCache { get; }
        public Formatter Formatter { get; }

        public LuaFeatureContainer()
        {
            ParseTreeCache = new ParseTreeCache();
            Formatter = new Formatter(ParseTreeCache);
        }
    }
}
