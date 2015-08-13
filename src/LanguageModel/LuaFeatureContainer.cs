using LanguageService.Diagnostics;
using LanguageService.Classification;
using LanguageService.Formatting;

namespace LanguageService
{
    public sealed class LuaFeatureContainer
    {
        private ParseTreeCache parseTreeCache;

        public IDiagnosticsProvider DiagnosticsProvider { get; }
        public Formatter Formatter { get; }
        public Colourizer Colourizer { get; }

        public LuaFeatureContainer()
        {
            this.parseTreeCache = new ParseTreeCache();
            this.Formatter = new Formatter(this.parseTreeCache);
            this.Colourizer = new Colourizer(this.parseTreeCache);
            this.DiagnosticsProvider = new DiagnosticsProvider(this.parseTreeCache);
        }
    }
}
