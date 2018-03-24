// Copyright (c) Microsoft. All rights reserved.

namespace LanguageModel.Tests.GeneratedTestFiles
{
    using LanguageModel.Tests.TestGeneration;
    using LanguageService;
    using Xunit;

    internal class EmbeddedFunction_Generated
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
                            t.N(SyntaxKind.NameVar);
                            {
                                t.N(SyntaxKind.Identifier);
                            }
                        }
                        t.N(SyntaxKind.AssignmentOperator);
                        t.N(SyntaxKind.ExpList);
                        {
                            t.N(SyntaxKind.FunctionDef);
                            {
                                t.N(SyntaxKind.FunctionKeyword);
                                t.N(SyntaxKind.FuncBodyNode);
                                {
                                    t.N(SyntaxKind.OpenParen);
                                    t.N(SyntaxKind.NameListPar);
                                    {
                                        t.N(SyntaxKind.NameList);
                                    }
                                    t.N(SyntaxKind.CloseParen);
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
                                                t.N(SyntaxKind.FunctionDef);
                                                {
                                                    t.N(SyntaxKind.FunctionKeyword);
                                                    t.N(SyntaxKind.FuncBodyNode);
                                                    {
                                                        t.N(SyntaxKind.OpenParen);
                                                        t.N(SyntaxKind.NameListPar);
                                                        {
                                                            t.N(SyntaxKind.NameList);
                                                        }
                                                        t.N(SyntaxKind.CloseParen);
                                                        t.N(SyntaxKind.BlockNode);
                                                        t.N(SyntaxKind.EndKeyword);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    t.N(SyntaxKind.EndKeyword);
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
