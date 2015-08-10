//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\FunctionCallStatement.lua\r\nusing LanguageModel.Tests.TestGeneration;
using LanguageService;
using LanguageModel.Tests.TestGeneration;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_13
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.FunctionCallStatementNode);
                    {
                        t.N(SyntaxKind.NameVar);
                        {
                            t.N(SyntaxKind.Identifier);
                        }
                        t.N(SyntaxKind.ParenArg);
                        {
                            t.N(SyntaxKind.OpenParen);
                            t.N(SyntaxKind.ExpList);
                            {
                                t.N(SyntaxKind.SimpleExpression);
                                {
                                    t.N(SyntaxKind.TrueKeyValue);
                                }
                                t.N(SyntaxKind.Comma);
                                t.N(SyntaxKind.SimpleExpression);
                                {
                                    t.N(SyntaxKind.Number);
                                }
                            }
                            t.N(SyntaxKind.CloseParen);
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
