//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\StringConcat.lua\r\nusing LanguageModel.Tests.TestGeneration;
using LanguageService;
using LanguageModel.Tests.TestGeneration;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_32
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
                                t.N(SyntaxKind.BinaryOperatorExpression);
                                {
                                    t.N(SyntaxKind.SimpleExpression);
                                    {
                                        t.N(SyntaxKind.String);
                                    }
                                    t.N(SyntaxKind.StringConcatOperator);
                                    t.N(SyntaxKind.SimpleExpression);
                                    {
                                        t.N(SyntaxKind.String);
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
