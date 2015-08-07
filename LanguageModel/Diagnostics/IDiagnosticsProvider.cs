using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Diagnostics
{
    /// <summary>
    /// The diagnostics interface that provides access to any diagnostics messages
    /// gathered by the language model or associated analyzers.
    /// </summary>
    public interface IDiagnosticsProvider
    {
        /// <summary>
        /// Gets the diagnostics for a given <paramref name="sourceText"/>. These messages
        /// may be generated from different sources, but are aggregated through this method. 
        /// At the moment, this list contains only parse errors (and the return type is constrained
        /// to parse errors, though it would be made a generic 'Message' in the future).
        /// </summary>
        /// <param name="sourceText">The source text to analyze and return messages for</param>
        /// <returns>The diagnostics for a given <paramref name="sourceText"/></returns>
        IReadOnlyList<ParseError> GetDiagnostics(SourceText sourceText);
    }
}
