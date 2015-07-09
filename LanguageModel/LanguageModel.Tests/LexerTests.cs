using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using Assert = Xunit.Assert;

namespace LanguageModel.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    public class LexerTests
    {
        [Fact]
        public void TestIfProgramTest()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\if.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            Assert.Equal(TokenType.IfKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EqualityOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ThenKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.PlusOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ElseIfKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EqualityOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ThenKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.MinusOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Colon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.SemiColon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Colon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.MinusOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Comma, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Colon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ElseIfKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EqualityOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ThenKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.MultiplyOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ElseIfKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EqualityOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ThenKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Colon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.MinusOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Comma, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Dot, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Colon, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.DivideOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.ElseKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.OpenParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.String, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.CloseParen, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndKeyword, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Identifier, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.AssignmentOperator, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.Number, tokenList[tokenIndex++].Type);
            Assert.Equal(TokenType.EndOfFile, tokenList[tokenIndex++].Type);
        }

        [Fact]
        public void TestAssignment()
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
        public void TestLeveledBlocks()
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
        public void TestLongCode()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\longcode.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void TestMaze()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\maze.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);
            int tokenIndex = 0;
            foreach (Token tok in tokenList)
            {
                Assert.NotEqual(TokenType.Unknown, tok.Type);
            }
        }

        [Fact]
        public void TestLongs()
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
        public void TestTabs()
        {
            Stream testProgramStream = File.OpenRead(@"CorrectSampleLuaFiles\tabs.lua");
            List<Token> tokenList = Lexer.Tokenize(testProgramStream);

            Assert.Equal(1, tokenList.Count);
            Assert.Equal("\t\t\t\t\t\t", tokenList[0].LeadingTrivia[0].Text);
            Assert.Equal(TokenType.EndOfFile, tokenList[0].Type);
        }

        [Fact]
        public void TestNewLines()
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

        public void TestPositionNumbers()
        {
            //TODO: implement
        }

    }
}