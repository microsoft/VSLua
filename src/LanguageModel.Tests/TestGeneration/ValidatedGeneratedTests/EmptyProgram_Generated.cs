// Copyright (c) Microsoft. All rights reserved.

namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class EmptyProgram_Generated
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
