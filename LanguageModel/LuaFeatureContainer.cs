/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using LanguageService.Classification;
using LanguageService.Diagnostics;
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
        /// Gets the Formatter object
        /// </summary>
        /// <value>The Formatter</value>
        public Formatter Formatter { get; }

        /// <summary>
        /// Gets the Colourizer object
        /// </summary>
        /// <value>The Colourizer</value>
        public Colourizer Colourizer { get; }
    }
}
