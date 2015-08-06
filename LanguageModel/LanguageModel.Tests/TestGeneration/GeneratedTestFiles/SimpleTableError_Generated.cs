using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class SimpleTableError_Generated
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
                            t.N(SyntaxKind.SeparatedListElement);
                            {
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                            }
                        }
                        t.N(SyntaxKind.AssignmentOperator);
                        t.N(SyntaxKind.ExpList);
                        {
                            t.N(SyntaxKind.SeparatedListElement);
                            {
                                t.N(SyntaxKind.Comma);
                                t.N(SyntaxKind.SimpleExpression);
                                {
                                    t.N(SyntaxKind.Number);
                                }
                            }
                            t.N(SyntaxKind.SeparatedListElement);
                            {
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                            }
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
