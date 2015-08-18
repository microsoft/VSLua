/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.IO;
using Validation;

namespace LanguageService
{
    public class SourceText
    {
        public SourceText(string text)
        {
            Requires.NotNull(text, nameof(text));
            this.text = text;
        }

        private string text;

        public TextReader TextReader => new StringReader(this.text);
    }
}
