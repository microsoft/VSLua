//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\TripleNestedFunctionCall.lua
namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class Generated_32
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
                                t.N(SyntaxKind.FunctionCallExp);
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
                                            t.N(SyntaxKind.FunctionCallExp);
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
                                                        t.N(SyntaxKind.NameVar);
                                                        {
                                                            t.N(SyntaxKind.Identifier);
                                                        }
                                                    }
                                                    t.N(SyntaxKind.CloseParen);
                                                }
                                            }
                                        }
                                        t.N(SyntaxKind.CloseParen);
                                    }
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
