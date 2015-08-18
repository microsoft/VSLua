//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\NestedFunctionCallStatement.lua
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_22
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
                                            t.N(SyntaxKind.BinaryOperatorExpression);
                                            {
                                                t.N(SyntaxKind.SimpleExpression);
                                                {
                                                    t.N(SyntaxKind.Number);
                                                }
                                                t.N(SyntaxKind.MultiplyOperator);
                                                t.N(SyntaxKind.SimpleExpression);
                                                {
                                                    t.N(SyntaxKind.Number);
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
