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

            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\if.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            int tokenIndex = 0;

            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.Equal(expectedTokens[i], tokenList[tokenIndex++].Type);
            }

        }

        [Fact]
        public void IdentifyAssignmentTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\assignment.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void IdentifyLeveledBlocksTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\leveled_blocks.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
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
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\concat.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
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
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            int tokenIndex = 0;
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void TestTrivia1()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\trivia1.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));

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
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\longcode.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void Comments1()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\comments1.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void NoUnknownTokensInCorrectLuaFile()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\maze.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void IdentifyLongsTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\longs.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
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
        public void IdentifyTabsTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\tabs.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));

            Assert.Equal(1, tokenList.Count);
            Assert.Equal("\t\t\t\t\t\t", tokenList[0].LeadingTrivia[0].Text);
            Assert.Equal(TokenType.EndOfFile, tokenList[0].Type);
        }

        [Fact]
        public void IdentifyNewLinesTokenTypes()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\newlines.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
            for (int triviaIndex = 0; triviaIndex < 8; triviaIndex++)
            {
                Assert.Equal(Trivia.TriviaType.Newline, tokenList[0].LeadingTrivia[triviaIndex].Type);
            }
            Assert.Equal(TokenType.EndOfFile, tokenList[0].Type);
        }

        public void TestSampleProgram()
        {
            TextReader testProgramStream = File.OpenText(@"CorrectSampleLuaFiles\test.lua");
            List<Token> tokenList = Lexer.Tokenize(new TrackableTextReader(testProgramStream));
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