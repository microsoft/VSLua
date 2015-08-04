using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;

namespace LanguageModel.Tests.GeneratedTestFiles
{
    class SmallIf_Generated
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
                                    t.N(SyntaxKind.CloseParen);
                                }
                            }
                            t.N(SyntaxKind.SemiColonStatementNode);
                            {
                                t.N(SyntaxKind.Semicolon);
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