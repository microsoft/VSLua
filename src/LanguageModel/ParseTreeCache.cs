/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Runtime.CompilerServices;
using Validation;

namespace LanguageService
{
    internal class ParseTreeCache
    {
        private readonly ConditionalWeakTable<SourceText, SyntaxTree> sources = 
            new ConditionalWeakTable<SourceText, SyntaxTree>();

        internal SyntaxTree Get(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            SyntaxTree syntaxTree;
            if (this.sources.TryGetValue(sourceText, out syntaxTree))
            {
                return syntaxTree;
            }

            syntaxTree = Parser.Parse(sourceText.TextReader);
            sources.Add(sourceText, syntaxTree);

            return syntaxTree;
        }
    }
}
