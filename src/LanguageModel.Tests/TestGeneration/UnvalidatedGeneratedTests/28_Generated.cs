//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\smallif.lua
namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class Generated_28
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
                            t.N(SyntaxKind.SimpleExpression);
                            {
                                t.N(SyntaxKind.TrueKeyValue);
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
                                    t.N(SyntaxKind.SquareBracketVar);
                                    {
                                        t.N(SyntaxKind.NameVar);
                                        {
                                            t.N(SyntaxKind.Identifier);
                                        }
                                        t.N(SyntaxKind.OpenBracket);
                                        t.N(SyntaxKind.SimpleExpression);
                                        {
                                            t.N(SyntaxKind.TrueKeyValue);
                                        }
                                        t.N(SyntaxKind.CloseBracket);
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
