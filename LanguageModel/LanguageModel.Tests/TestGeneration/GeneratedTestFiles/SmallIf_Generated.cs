using LanguageModel.Tests.TestGeneration;
using LanguageService;

namespace LanguageModel.Tests.GeneratedTestFiles
{
    class SmallIf_Generated
    {
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
                }
              }
            }
        }
    }
}
