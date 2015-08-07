using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService.Diagnostics;
using LanguageService.Formatting;

namespace LanguageService
{
    public sealed class LuaFeatureContainer
    {
        private ParseTreeCache parseTreeCache;

        public IDiagnosticsProvider DiagnosticsProvider { get; }
        public Formatter Formatter { get; }

        public LuaFeatureContainer()
        {
            this.parseTreeCache = new ParseTreeCache();

            this.DiagnosticsProvider = new DiagnosticsProvider(this.parseTreeCache);
            this.Formatter = new Formatter(parseTreeCache);
        }
    }
}
