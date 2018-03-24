//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\GenericForStatement.lua
namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class Generated_15
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.MultipleArgForStatementNode);
                    {
                        t.N(SyntaxKind.ForKeyword);
                        t.N(SyntaxKind.NameList);
                        {
                            t.N(SyntaxKind.Identifier);
                            t.N(SyntaxKind.Comma);
                            t.N(SyntaxKind.Identifier);
                        }
                        t.N(SyntaxKind.InKeyword);
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
                        t.N(SyntaxKind.DoKeyword);
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
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                    }
                                    t.N(SyntaxKind.CloseParen);
                                }
                            }
                        }
                        t.N(SyntaxKind.EndKeyword);
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
