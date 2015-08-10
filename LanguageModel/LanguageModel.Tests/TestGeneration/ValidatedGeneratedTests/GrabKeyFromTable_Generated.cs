using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class GrabKeyFromTable_Generated
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
                            t.N(SyntaxKind.SquareBracketVar);
                            {
                                t.N(SyntaxKind.NameVar);
                                {
                                    t.N(SyntaxKind.Identifier);
                                }
                                t.N(SyntaxKind.OpenBracket);
                                t.N(SyntaxKind.SimpleExpression);
                                {
                                    t.N(SyntaxKind.String);
                                }
                                t.N(SyntaxKind.CloseBracket);
                            }
                        }
                        t.N(SyntaxKind.MissingToken);
                        t.N(SyntaxKind.ExpList);
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
