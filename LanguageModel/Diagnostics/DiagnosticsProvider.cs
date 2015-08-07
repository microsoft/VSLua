using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace LanguageService.Diagnostics
{
    internal class DiagnosticsProvider : IDiagnosticsProvider
    {
        private ParseTreeCache parseTreeCache;

        public DiagnosticsProvider(ParseTreeCache parseTreeCache)
        {
            Requires.NotNull(parseTreeCache, nameof(parseTreeCache));

            this.parseTreeCache = parseTreeCache;
        }

        public IReadOnlyList<ParseError> GetDiagnostics(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            SyntaxTree syntaxTree = this.parseTreeCache.Get(sourceText);

            return syntaxTree.ErrorList;
        }
    }
}
