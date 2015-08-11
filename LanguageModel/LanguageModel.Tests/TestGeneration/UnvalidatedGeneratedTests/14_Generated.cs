//C:\Users\t-kevimi\\Documents\\LuaTests\Lua Files for Testing\FunctionCallWithTableConstructor.lua
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class Generated_14
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
                        t.N(SyntaxKind.TableConstructorArg);
                        {
                            t.N(SyntaxKind.OpenCurlyBrace);
                            t.N(SyntaxKind.FieldList);
                            {
                                t.N(SyntaxKind.AssignmentField);
                                {
                                    t.N(SyntaxKind.Identifier);
                                    t.N(SyntaxKind.AssignmentOperator);
                                    t.N(SyntaxKind.SimpleExpression);
                                    {
                                        t.N(SyntaxKind.String);
                                    }
                                }
                                t.N(SyntaxKind.Comma);
                                t.N(SyntaxKind.AssignmentField);
                                {
                                    t.N(SyntaxKind.Identifier);
                                    t.N(SyntaxKind.AssignmentOperator);
                                    t.N(SyntaxKind.SimpleExpression);
                                    {
                                        t.N(SyntaxKind.String);
                                    }
                                }
                            }
                            t.N(SyntaxKind.CloseCurlyBrace);
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
