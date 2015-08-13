using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class EmptyProgram_Generated
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
