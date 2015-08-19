
using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;
namespace LanguageModel.Tests.GeneratedTestFiles
{
    class NestedTable_Generated
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
                            t.N(SyntaxKind.TableConstructorExp);
                            {
                                t.N(SyntaxKind.OpenCurlyBrace);
                                t.N(SyntaxKind.FieldList);
                                {
                                    t.N(SyntaxKind.AssignmentField);
                                    {
                                        t.N(SyntaxKind.Identifier);
                                        t.N(SyntaxKind.AssignmentOperator);
                                        t.N(SyntaxKind.TableConstructorExp);
                                        {
                                            t.N(SyntaxKind.OpenCurlyBrace);
                                            t.N(SyntaxKind.FieldList);
                                            t.N(SyntaxKind.CloseCurlyBrace);
                                        }
                                    }
                                }
                                t.N(SyntaxKind.CloseCurlyBrace);
                            }
                        }
                    }
                }
                t.N(SyntaxKind.EndOfFile);
            }
        }
    }
}
