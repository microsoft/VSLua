using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var expectedTokens = new TokenType[]
            {
                TokenType.IfKeyword,
                TokenType.Identifier,
                TokenType.EqualityOperator,
                TokenType.String,
                TokenType.ThenKeyword,
                TokenType.Identifier,
                TokenType.AssignmentOperator,
                TokenType.Identifier,
                TokenType.PlusOperator,
                TokenType.Identifier,
                TokenType.ElseIfKeyword,
                TokenType.Identifier,
                TokenType.EqualityOperator,
                TokenType.String,
                TokenType.ThenKeyword,
                TokenType.Identifier,
                TokenType.AssignmentOperator,
                TokenType.Identifier,
                TokenType.MinusOperator,
                TokenType.Identifier,
                TokenType.Identifier,
                TokenType.Colon,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.Identifier,
                TokenType.CloseParen,
                TokenType.SemiColon,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.Colon,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.MinusOperator,
                TokenType.Number,
                TokenType.Comma,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.CloseParen,
                TokenType.Colon,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.Identifier,
                TokenType.CloseParen,
                TokenType.ElseIfKeyword,
                TokenType.Identifier,
                TokenType.EqualityOperator,
                TokenType.String,
                TokenType.ThenKeyword,
                TokenType.Identifier,
                TokenType.AssignmentOperator,
                TokenType.Identifier,
                TokenType.MultiplyOperator,
                TokenType.Identifier,
                TokenType.ElseIfKeyword,
                TokenType.Identifier,
                TokenType.EqualityOperator,
                TokenType.String,
                TokenType.ThenKeyword,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.Colon,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.MinusOperator,
                TokenType.Number,
                TokenType.Comma,
                TokenType.Identifier,
                TokenType.Dot,
                TokenType.Identifier,
                TokenType.CloseParen,
                TokenType.Colon,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.Identifier,
                TokenType.CloseParen,
                TokenType.Identifier,
                TokenType.AssignmentOperator,
                TokenType.Identifier,
                TokenType.DivideOperator,
                TokenType.Identifier,
                TokenType.ElseKeyword,
                TokenType.Identifier,
                TokenType.OpenParen,
                TokenType.String,
                TokenType.CloseParen,
                TokenType.EndKeyword,
                TokenType.Identifier,
                TokenType.AssignmentOperator,
                TokenType.Number,
                TokenType.EndOfFile,
            };

            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\if.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;

            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.Equal(expectedTokens[i], tokenList[tokenIndex++].Type);
            }

        }

        [Fact]
        public void IdentifyAssignmentTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\assignment.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void IdentifyLeveledBlocksTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\leveled_blocks.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void TestConcat()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\concat.lua");
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
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\longcomment.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void TestTrivia1()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\trivia1.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);

            List<Trivia.TriviaType> allTrivia = new List<Trivia.TriviaType>();

            foreach (Token token in tokenList)
            {
                foreach (Trivia trivia in token.LeadingTrivia)
                {
                    allTrivia.Add(trivia.Type);
                }
            }

            List<Trivia.TriviaType> compareTrivia = new List<Trivia.TriviaType>
            {
                Trivia.TriviaType.Whitespace,
                Trivia.TriviaType.Newline,
                Trivia.TriviaType.Newline,
                Trivia.TriviaType.Whitespace,
                Trivia.TriviaType.Newline,
                Trivia.TriviaType.Whitespace,
            };

            Assert.Equal(compareTrivia, allTrivia);




        }

        [Fact]
        public void IdentifyLongCodeTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\longcode.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void NoUnknownTokensInCorrectLuaFile()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\maze.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void IdentifyLongsTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\longs.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);

            for (int j = 0; j < 62; j++)
            {
                Assert.Equal(TokenType.CloseBracket, tokenList[tokenIndex++].Type);
            }

            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void IdnetifyTabsTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\tabs.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);

            Assert.Equal(1, tokenList.Count);
            Assert.Equal("\t\t\t\t\t\t", tokenList[0].LeadingTrivia[0].Text);
            Assert.Equal(TokenType.EndOfFile, tokenList[0].Type);
        }

        [Fact]
        public void IdentifyNewLinesTokenTypes()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\newlines.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            for (int triviaIndex = 0; triviaIndex < 8; triviaIndex++)
            {
                Assert.Equal(Trivia.TriviaType.Newline, tokenList[0].LeadingTrivia[triviaIndex].Type);
            }
            Assert.Equal(TokenType.EndOfFile, tokenList[0].Type);
        }

        public void TestSampleProgram()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\test.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.EndKeyword, tokenList[tokenIndex++].Type);
            int triviaIndex = 0;
            Assert.Equal(Trivia.TriviaType.Newline, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(Trivia.TriviaType.Newline, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(Trivia.TriviaType.Comment, tokenList[tokenIndex].LeadingTrivia[triviaIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
        }
    }
}