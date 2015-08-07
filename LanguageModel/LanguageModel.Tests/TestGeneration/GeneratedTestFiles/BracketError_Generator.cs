using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class BracketsError_Generated
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
                            t.N(SyntaxKind.DotVar);
                            {
                                t.N(SyntaxKind.ParenPrefixExp);
                                {
                                    t.N(SyntaxKind.OpenParen);
                                    t.N(SyntaxKind.MissingToken);
                                    {
                                        t.N(SyntaxKind.MissingToken);
                                    }
                                    t.N(SyntaxKind.MissingToken);
                                }
                                t.N(SyntaxKind.MissingToken);
                                t.N(SyntaxKind.MissingToken);
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
