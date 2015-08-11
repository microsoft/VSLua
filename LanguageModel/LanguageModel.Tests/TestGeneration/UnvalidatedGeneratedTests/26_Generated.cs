//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\ReturnStatementInFunction.lua
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_26
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.GlobalFunctionStatementNode);
                    {
                        t.N(SyntaxKind.FunctionKeyword);
                        t.N(SyntaxKind.FuncNameNode);
                        {
                            t.N(SyntaxKind.Identifier);
                            t.N(SyntaxKind.DotSeparatedNameList);
                        }
                        t.N(SyntaxKind.FuncBodyNode);
                        {
                            t.N(SyntaxKind.OpenParen);
                            t.N(SyntaxKind.NameListPar);
                            {
                                t.N(SyntaxKind.NameList);
                            }
                            t.N(SyntaxKind.CloseParen);
                            t.N(SyntaxKind.BlockNode);
                            {
                                t.N(SyntaxKind.ReturnStatementNode);
                                {
                                    t.N(SyntaxKind.ReturnKeyword);
                                    t.N(SyntaxKind.ExpList);
                                    t.N(SyntaxKind.Semicolon);
                                }
                            }
                            t.N(SyntaxKind.EndKeyword);
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
