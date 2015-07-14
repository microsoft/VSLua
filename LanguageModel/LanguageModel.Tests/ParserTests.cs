using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using Assert = Xunit.Assert;

namespace LanguageModel.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    public class ParserTests
    {
        [Fact]
        public void testParser()
        {
            Parser parser = new Parser();
            SyntaxTree tree = parser.CreateSyntaxTree(@"CorrectSampleLuaFiles\smallif.lua");
            Assert.Equal(tree.Root.ProgramBlock.ReturnStatement, null);
            Assert.Equal(1, tree.Root.ProgramBlock.Children.Count);
            Assert.Equal(tree.Root.ProgramBlock.Children[0] is IfNode, true);
            Assert.Equal((tree.Root.ProgramBlock.Children[0] as IfNode).IfBlock.Children.Count, 2);

        }

    }
}
