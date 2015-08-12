using LanguageService.Diagnostics;
using LanguageService.Classification;
using LanguageService.Formatting;

namespace LanguageService
{
    /// <summary>
    /// Contains all the Lua language service features
    /// </summary>
    public sealed class LuaFeatureContainer
    {
        private ParseTreeCache parseTreeCache;

        public LuaFeatureContainer()
        {
            this.parseTreeCache = new ParseTreeCache();
            this.Formatter = new Formatter(this.parseTreeCache);
            this.Colourizer = new Colourizer(this.parseTreeCache);
            this.DiagnosticsProvider = new DiagnosticsProvider(this.parseTreeCache);
        }

        public IDiagnosticsProvider DiagnosticsProvider { get; }

        /// <summary>
        /// Gets the formatting object
        /// </summary>
        public Formatter Formatter { get; }

        public Colourizer Colourizer { get; }
    }
}
