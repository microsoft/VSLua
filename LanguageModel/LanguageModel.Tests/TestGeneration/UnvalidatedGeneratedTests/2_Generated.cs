//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\AssignmentWithFunctionCall.lua\r\nusing LanguageModel.Tests.TestGeneration;
using LanguageService;
using LanguageModel.Tests.TestGeneration;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_2
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.AssignmentStatementNode);
                    {
                        t.N(SyntaxKind.VarList);
                        {
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                        }
                        t.N(SyntaxKind.AssignmentOperator);
                        t.N(SyntaxKind.ExpList);
                        {
                            t.N(SyntaxKind.DotVar);
                            {
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                                t.N(SyntaxKind.Dot);
                                t.N(SyntaxKind.Identifier);
                            }
                        }
                    }
                    t.N(SyntaxKind.FunctionCallStatementNode);
                    {
                        t.N(SyntaxKind.ParenPrefixExp);
                        {
                            t.N(SyntaxKind.OpenParen);
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                            t.N(SyntaxKind.MissingToken);
                        }
                        t.N(SyntaxKind.MissingToken);
                        {
                            t.N(SyntaxKind.MissingToken);
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
