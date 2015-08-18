//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\BreakIfStatement.lua
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_3
    {
        [Fact]
        public void Test(Tester t)
        {
            t.N(SyntaxKind.ChunkNode);
            {
                t.N(SyntaxKind.BlockNode);
                {
                    t.N(SyntaxKind.IfStatementNode);
                    {
                        t.N(SyntaxKind.IfKeyword);
                        t.N(SyntaxKind.BinaryOperatorExpression);
                        {
                            t.N(SyntaxKind.SquareBracketVar);
                            {
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                                t.N(SyntaxKind.OpenBracket);
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                                t.N(SyntaxKind.CloseBracket);
                            }
                            t.N(SyntaxKind.EqualityOperator);
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                        }
                        t.N(SyntaxKind.ThenKeyword);
                        t.N(SyntaxKind.BlockNode);
                        {
                            t.N(SyntaxKind.BreakStatementNode);
                            {
                                t.N(SyntaxKind.BreakKeyword);
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
