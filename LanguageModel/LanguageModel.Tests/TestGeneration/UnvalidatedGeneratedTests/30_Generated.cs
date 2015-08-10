//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\SemiColonStatement.lua\r\nusing LanguageModel.Tests.TestGeneration;
using LanguageService;
using LanguageModel.Tests.TestGeneration;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_30
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.SemiColonStatementNode);
                    {
                        t.N(SyntaxKind.Semicolon);
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
