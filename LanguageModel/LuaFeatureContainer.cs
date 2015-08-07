using LanguageService.Classification;
using LanguageService.Formatting;

namespace LanguageService
{
    public sealed class LuaFeatureContainer
    {
        internal ParseTreeCache ParseTreeCache { get; }
        public Formatter Formatter { get; }
        public Colourizer Colourizer { get; }

        public LuaFeatureContainer()
        {
            this.ParseTreeCache = new ParseTreeCache();
            this.Formatter = new Formatter(ParseTreeCache);
            this.Colourizer = new Colourizer(ParseTreeCache);
        }
    }
}
