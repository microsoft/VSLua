using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Xunit;
using LanguageService;
using Assert = Xunit.Assert;
using Xunit.Abstractions;

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


        [Fact]
        public void testSmallSampleIf()
        {
            Parser parser = new Parser();
            SyntaxTree tree = parser.CreateSyntaxTree(@"CorrectSampleLuaFiles\smallif.lua");

            //SyntaxTree expected = ChunkNode.Create(0, 10,
            //    Block.Create(0, 0, ImmutableList.Create<SyntaxNode>(
            //        IfNode.Create(

            //        )));
            //index/#Microsoft.VisualStudio.Composition/IndentingTextWriter.cs,083b8cac8f07e6cd 
            //File: IndentingTextWriter.cs
            //Project: Microsoft.VisualStudio.Composition\Microsoft.VisualStudio.Composition.csproj(Microsoft.VisualStudio.Composition)
            // DiffPlex


            Assert.Equal(tree.Root.ProgramBlock.ReturnStatement, null);
            Assert.Equal(1, tree.Root.ProgramBlock.Children.Count);
            Assert.Equal(tree.Root.ProgramBlock.Children[0] is IfNode, true);
            this.logger.WriteLine("blahblah");
            Assert.Equal((tree.Root.ProgramBlock.Children[0] as IfNode).Exp is BinopExpression, true);
            Assert.Equal(((tree.Root.ProgramBlock.Children[0] as IfNode).Exp as BinopExpression).Binop.Type, TokenType.EqualityOperator);
            Assert.Equal((tree.Root.ProgramBlock.Children[0] as IfNode).IfBlock.Children.Count, 2);
            Assert.Equal((tree.Root.ProgramBlock.Children[0] as IfNode).EndKeyword.Type, TokenType.EndKeyword);
            Assert.Equal((tree.Root.EndOfFile as Token).Type, TokenType.EndOfFile);
        }

    }
}
