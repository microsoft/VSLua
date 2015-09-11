/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Runtime.CompilerServices;
using Validation;

namespace LanguageService
{
    public class ParseTreeCache
    {
        private readonly ConditionalWeakTable<SourceText, SyntaxTree> sources = 
            new ConditionalWeakTable<SourceText, SyntaxTree>();

        public SyntaxTree Get(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            SyntaxTree syntaxTree;
            if (this.sources.TryGetValue(sourceText, out syntaxTree))
            {
                return syntaxTree;
            }

            syntaxTree = Parser.Parse(sourceText.TextReader);
            this.sources.Add(sourceText, syntaxTree);

            return syntaxTree;
        }
    }
}
