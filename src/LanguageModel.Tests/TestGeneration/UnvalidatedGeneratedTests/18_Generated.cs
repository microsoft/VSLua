//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\IfStatement.lua
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_18
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
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                            t.N(SyntaxKind.EqualityOperator);
                            t.N(SyntaxKind.SimpleExpression);
                            {
                                t.N(SyntaxKind.String);
                            }
                        }
                        t.N(SyntaxKind.ThenKeyword);
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
                                    t.N(SyntaxKind.BinaryOperatorExpression);
                                    {
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                        t.N(SyntaxKind.PlusOperator);
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                    }
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
