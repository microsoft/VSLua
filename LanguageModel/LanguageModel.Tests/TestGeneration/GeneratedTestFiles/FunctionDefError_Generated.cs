using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;

namespace LanguageModel.Tests.GeneratedTestFiles
{
    class FunctionDefError_Generated
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
                                t.N(SyntaxKind.FunctionDef);
                                {
                                    t.N(SyntaxKind.FunctionKeyword);
                                    t.N(SyntaxKind.FuncBodyNode);
                                    {
                                        t.N(SyntaxKind.MissingToken);
                                        t.N(SyntaxKind.NameListPar);
                                        {
                                            t.N(SyntaxKind.NameList);
                                        }
                                        t.N(SyntaxKind.MissingToken);
                                        t.N(SyntaxKind.BlockNode);
                                        {
                                            t.N(SyntaxKind.ReturnStatementNode);
                                            {
                                                t.N(SyntaxKind.ReturnKeyword);
                                                t.N(SyntaxKind.ExpList);
                                            }
                                        }
                                        t.N(SyntaxKind.EndKeyword);
                                    }
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