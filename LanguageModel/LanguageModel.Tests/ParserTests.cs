using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using Assert = Xunit.Assert;

namespace LanguageModel.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    class ParserTests
    {
        [Fact]
        public void testParser()
        {
            Parser parser = new Parser();
            SyntaxTree tree = parser.CreateSyntaxTree(@"CorrectSampleLuaFiles\smallif.lua");
            Assert.Equal(tree.Root.ProgramBlock.ReturnStatement, null);

        }

    }
}
