using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;

using Assert = Xunit.Assert;

namespace LanguageService.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    public class LexerTests
    {
        [Fact]
        public void IdentifyCorrectTokenTypes()
        {
            var expectedTokens = new SyntaxKind[]
            {
                SyntaxKind.IfKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.EqualityOperator,
                SyntaxKind.String,
                SyntaxKind.ThenKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Identifier,
                SyntaxKind.PlusOperator,
                SyntaxKind.Identifier,
                SyntaxKind.ElseIfKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.EqualityOperator,
                SyntaxKind.String,
                SyntaxKind.ThenKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Identifier,
                SyntaxKind.MinusOperator,
                SyntaxKind.Identifier,
                SyntaxKind.Identifier,
                SyntaxKind.Colon,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.Identifier,
                SyntaxKind.CloseParen,
                SyntaxKind.Semicolon,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.Colon,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.MinusOperator,
                SyntaxKind.Number,
                SyntaxKind.Comma,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.CloseParen,
                SyntaxKind.Colon,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.Identifier,
                SyntaxKind.CloseParen,
                SyntaxKind.ElseIfKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.EqualityOperator,
                SyntaxKind.String,
                SyntaxKind.ThenKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Identifier,
                SyntaxKind.MultiplyOperator,
                SyntaxKind.Identifier,
                SyntaxKind.ElseIfKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.EqualityOperator,
                SyntaxKind.String,
                SyntaxKind.ThenKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.Colon,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.MinusOperator,
                SyntaxKind.Number,
                SyntaxKind.Comma,
                SyntaxKind.Identifier,
                SyntaxKind.Dot,
                SyntaxKind.Identifier,
                SyntaxKind.CloseParen,
                SyntaxKind.Colon,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.Identifier,
                SyntaxKind.CloseParen,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Identifier,
                SyntaxKind.DivideOperator,
                SyntaxKind.Identifier,
                SyntaxKind.ElseKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.OpenParen,
                SyntaxKind.String,
                SyntaxKind.CloseParen,
                SyntaxKind.EndKeyword,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Number,
                SyntaxKind.EndOfFile,
            };

            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\if.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;

            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.Equal(expectedTokens[i], tokenList[tokenIndex++].Kind);
            }

        }

        [Fact]
        public void IdentifyAssignmentTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\assignment.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(SyntaxKind.Identifier, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.AssignmentOperator, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.Number, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.EndOfFile, tokenList[tokenIndex++].Kind);
        }

        [Fact]
        public void IdentifyLeveledBlocksTokenTypes()
        {
            var expectedTokens = new SyntaxKind[]
            {
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.String,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.String,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.Unknown,
                SyntaxKind.Identifier,
                SyntaxKind.Identifier,
                SyntaxKind.AssignmentOperator,
                SyntaxKind.UnterminatedString,
                SyntaxKind.EndOfFile,
            };

            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\leveled_blocks.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;

            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.Equal(expectedTokens[i], tokenList[tokenIndex++].Kind);
            }
        }

        [Fact]
        public void TestConcat()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\concat.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            List<string> actual = new List<string>();

            foreach (Token token in tokenList)
            {
                actual.Add(token.Text);
            }


            List<string> compare = new List<string>
            {
                "..",
                ""
            };

            Assert.Equal(compare, actual);

        }

        [Fact]
        public void TestLongCommentOnSameLine()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\longcomment.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(SyntaxKind.Identifier, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.EndOfFile, tokenList[tokenIndex++].Kind);
        }

        [Fact]
        public void TestTrivia1()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\trivia1.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);

            List<SyntaxKind> allTrivia = new List<SyntaxKind>();

            foreach (Token token in tokenList)
            {
                foreach (Trivia trivia in token.LeadingTrivia)
                {
                    allTrivia.Add(trivia.Type);
                }
            }

            List<SyntaxKind> compareTrivia = new List<SyntaxKind>
            {
                SyntaxKind.Whitespace,
                SyntaxKind.Newline,
                SyntaxKind.Newline,
                SyntaxKind.Whitespace,
                SyntaxKind.Newline,
                SyntaxKind.Whitespace,
            };

            Assert.Equal(compareTrivia, allTrivia);
        }

        [Fact]
        public void IdentifyLongCodeTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\longcode.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            foreach (Token tok in tokenList)
            {
                Debug.WriteLine(tok.ToString());
                Assert.NotEqual(SyntaxKind.Unknown, tok.Kind);
            }
        }

        [Fact]
        public void NoUnknownTokensInCorrectLuaFile()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\maze.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            foreach (Token tok in tokenList)
            {
                Debug.WriteLine(tok.ToString());
                Assert.NotEqual(SyntaxKind.Unknown, tok.Kind);
            }
        }

        [Fact]
        public void IdentifyLongsTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\longs.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(SyntaxKind.Identifier, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.Number, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.String, tokenList[tokenIndex++].Kind);

            for (int j = 0; j < 62; j++)
            {
                Assert.Equal(SyntaxKind.CloseBracket, tokenList[tokenIndex++].Kind);
            }

            Assert.Equal(SyntaxKind.String, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.EndOfFile, tokenList[tokenIndex++].Kind);
        }

        [Fact]
        public void IdentifyTabsTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\tabs.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);

            Assert.Equal(1, tokenList.Count);
            Assert.Equal("\t\t\t\t\t\t", tokenList[0].LeadingTrivia[0].Text);
            Assert.Equal(SyntaxKind.EndOfFile, tokenList[0].Kind);
        }

        [Fact]
        public void IdentifyNewLinesTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\newlines.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            for (int triviaIndex = 0; triviaIndex < 8; triviaIndex++)
            {
                Assert.Equal(SyntaxKind.Newline, tokenList[0].LeadingTrivia[triviaIndex].Type);
            }
            Assert.Equal(SyntaxKind.EndOfFile, tokenList[0].Kind);
        }

        public void TestSampleProgram()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\test.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(SyntaxKind.EndKeyword, tokenList[tokenIndex++].Kind);
            int triviaIndex = 0;
            Assert.Equal(SyntaxKind.Newline, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(SyntaxKind.Newline, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(SyntaxKind.Comment, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(SyntaxKind.Identifier, tokenList[tokenIndex++].Kind);
            Assert.Equal(SyntaxKind.AssignmentOperator, tokenList[tokenIndex++].Kind);
        }
    }
}