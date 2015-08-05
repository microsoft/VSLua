using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;

namespace LanguageModel.Tests.GeneratedTestFiles
{
    class TripleNestedFunctionCall_Generated
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
                                t.N(SyntaxKind.SeparatedListElement);
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
                                                t.N(SyntaxKind.SeparatedListElement);
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
                                                                t.N(SyntaxKind.SeparatedListElement);
                                                                {
                                                                    t.N(SyntaxKind.NameVar);
                                                                    {
                                                                        t.N(SyntaxKind.Identifier);
                                                                    }
                                                                }
                                                            }
                                                            t.N(SyntaxKind.CloseParen);
                                                        }
                                                    }
                                                }
                                            }
                                            t.N(SyntaxKind.CloseParen);
                                        }
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