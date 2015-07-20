using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace LanguageService.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    public class ParserTests
    {
        private ITestOutputHelper logger;

        public ParserTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        private TextReader ReadTestSource(string file)
        {
            using (var sr = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream($"LanguageModel.Tests.{file.Replace('\\','.')}")))
            {
                return new StringReader(sr.ReadToEnd());
            }
        }

        //private SyntaxTree GetSampleSyntaxTree(string file)
        //{
        //    Parser parser = new Parser();
        //    return parser.CreateSyntaxTree(ReadTestSource(file));
        //}

        //private void AssertExpectedTree(SyntaxTree actual, string expectedAnswerFilePath)
        //{
        //    string actualAsJson = SerializeAsJson(actual);
        //    DiffPlex.Diff(actualAsJson, ReadTestSource(expectedAnswerFilePath));

        //}

        [Fact]
        public void testNestedSampleIf()
        {
            //SyntaxTree tree = GetSampleSyntaxTree(@"CorrectSampleLuaFiles\nestedif.lua");

            Parser parser = new Parser();
            SyntaxTree tree = parser.CreateSyntaxTree(@"CorrectSampleLuaFiles\nestedif.lua");

            //The expected tree ignores Trivia, hence all the null parameteres
            //TODO: complete construction of neste
            SyntaxTree expected = new SyntaxTree("nestedif.lua",
                ChunkNode.Create(0, 247,
                    BlockNode.Create(0, 244, new List<SyntaxNode>()
                    {
                        IfStatementNode.Create(2, 117,
                            new Token(TokenType.IfKeyword, "if", null, 0, 2),
                            BinaryOperatorExpression.Create(5, 11,
                                SimpleExpression.Create(5, 4, new Token(TokenType.TrueKeyValue, "true", null, 4, 5)),
                                new Token(TokenType.EqualityOperator, "==", null, 9, 10),
                                SimpleExpression.Create(13, 3, new Token(TokenType.String, "\"+\"", null, 12, 13))),
                            new Token(TokenType.ThenKeyword, "then", null, 16, 17),
                            BlockNode.Create(26, 5/*TODO: change once expanded*/, new List<SyntaxNode>()
                            {
                                MisplacedTokenNode.Create(26, 4, new Token(TokenType.Identifier, "test", null, 21, 26)),
                                SemiColonStatementNode.Create(30, 1, new Token(TokenType.Semicolon, ";", null, 30, 30))
                            }.ToImmutableList()),
                            null,
                            new Token(TokenType.EndKeyword,"end",null,31,32))
                    }.ToImmutableList()),
                    new Token(TokenType.EndOfFile, "", null, 35, 35)),/*TODO: change once expanded*/
                    null);

            Assert.Equal(expected.ToString(), tree.ToString());
        }

        [Fact]
        public void testSmallSampleIf()
        {
            Parser parser = new Parser();
            SyntaxTree tree = parser.CreateSyntaxTree(@"CorrectSampleLuaFiles\smallif.lua");

            //TODO: find more maintainable way to add position numbers...
            SyntaxTree expected = new SyntaxTree("smallif.lua",
                ChunkNode.Create(0, 247,
                    BlockNode.Create(0, 244, new List<SyntaxNode>()
                    {
                        IfStatementNode.Create(2, 117,
                            new Token(TokenType.IfKeyword, "if", null, 0, 2),
                            BinaryOperatorExpression.Create(5, 11,
                                SimpleExpression.Create(5, 4, new Token(TokenType.TrueKeyValue, "true", null, 4, 5)),
                                new Token(TokenType.EqualityOperator, "==", null, 9, 10),
                                SimpleExpression.Create(13, 3, new Token(TokenType.String, "\"+\"", null, 12, 13))),
                            new Token(TokenType.ThenKeyword, "then", null, 16, 17),
                            BlockNode.Create(26, 5/*TODO: change once expanded*/, new List<SyntaxNode>()
                            {
                                MisplacedTokenNode.Create(26, 4, new Token(TokenType.Identifier, "test", null, 21, 26)),
                                SemiColonStatementNode.Create(30, 1, new Token(TokenType.Semicolon, ";", null, 30, 30))
                            }.ToImmutableList()),
                            null,
                            new Token(TokenType.EndKeyword,"end",null,31,32))
                    }.ToImmutableList()),
                    new Token(TokenType.EndOfFile, "", null, 35, 35)),/*TODO: change once expanded*/
                    null);

            Debug.WriteLine(tree.ToString());
            Debug.WriteLine(expected.ToString());

            Assert.Equal(expected.ToString(), tree.ToString());

            //TODO: refactor  test code
            Assert.Equal(tree.Root.ProgramBlock.ReturnStatement, null);
            Assert.Equal(1, tree.Root.ProgramBlock.Statements.Count);
            Assert.Equal(tree.Root.ProgramBlock.Statements[0] is IfStatementNode, true);
            Assert.Equal((tree.Root.ProgramBlock.Statements[0] as IfStatementNode).Exp is BinaryOperatorExpression, true);
            Assert.Equal(((tree.Root.ProgramBlock.Statements[0] as IfStatementNode).Exp as BinaryOperatorExpression).BinaryOperator.Type, TokenType.EqualityOperator);
            Assert.Equal((tree.Root.ProgramBlock.Statements[0] as IfStatementNode).IfBlock.Statements.Count, 2);
            Assert.Equal((tree.Root.ProgramBlock.Statements[0] as IfStatementNode).EndKeyword.Type, TokenType.EndKeyword);
            Assert.Equal((tree.Root.EndOfFile as Token).Type, TokenType.EndOfFile);

            Assert.Equal(expected.ToString(), tree.ToString());
        }

    }
}
