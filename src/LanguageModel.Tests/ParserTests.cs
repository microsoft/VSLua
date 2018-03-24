// Copyright (c) Microsoft. All rights reserved.

namespace LanguageService.Tests
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using global::LanguageModel.Tests;
    using global::LanguageModel.Tests.GeneratedTestFiles;
    using global::LanguageModel.Tests.TestGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Xunit;
    using Assert = Xunit.Assert;

    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    [DeploymentItem("SerializedJsonOutput", "SerializedJsonOutput")]
    public class ParserTests
    {
        [Fact]
        public void SmallIfGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\smallif.lua");
            new SmallIf_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void AssignmentsGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\Assignments.lua");
            new Assignments_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void MultipleTypeAssignmentGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\MultipleTypeAssignment.lua");
            new MultipleTypeAssignment_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void WhileStatementGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\WhileStatement.lua");
            new WhileStatement_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void ComplexTableConstructorGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\ComplexTableConstructor.lua");
            new ComplexTableConstructor_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void TableStatementsGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\TableStatements.lua");
            new TableStatements_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void TripleNestedFunctionCallGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\TripleNestedFunctionCall.lua");
            new TripleNestedFunctionCall_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void FunctionDefErrorGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\FunctionDefError.lua");
            new FunctionDefError_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void SimpleTableGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("{ x=1, y=2 }");
            new SimpleTableError_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void LucaDemoGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString(@"
x= 1
-- this is an add function
add = function(x, y)
    return x+y;-- adding
end


get_zero = function() return 0 end");

            var generator = new TestGenerator();
            generator.GenerateTestFromString(@"
x= 1
-- this is an add function
add = function(x, y)
    return x+y;-- adding
end


get_zero = function() return 0 end", "LucaDemo");
            Debug.WriteLine(tree.ErrorList.Count);

            new LucaDemo_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void GrabKeyFromTableGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("t[\"this is a test that grabs this key in Lua\"]");
            var generator = new TestGenerator();
            generator.GenerateTestFromString("t[\"this is a test that grabs this key in Lua\"]", "GrabKeyFromTable");
            new GrabKeyFromTable_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void EmptyProgramGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("");
            var generator = new TestGenerator();
            generator.GenerateTestFromString("", "EmptyProgram");
            new EmptyProgram_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void BracketsErrorGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("}(");
            var generator = new TestGenerator();
            generator.GenerateTestFromString("}(", "BracketsError");
            new BracketsError_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void PrefixExpFirstGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("(f)[s] = k");
            var generator = new TestGenerator();
            generator.GenerateTestFromString("(f)[s] = k", "PrefixExpFirst");
            new PrefixExpFirst_Generated().Test(new Tester(tree));
            Assert.Equal(0, tree.ErrorList.Count);
        }

        [Fact]
        public void PrefixExpNestedTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString("foo.bar(t[5]).set():run{}");
            Assert.Equal(0, tree.ErrorList.Count);
            var generator = new TestGenerator();
            generator.GenerateTestFromString("foo.bar(t[5]).set():run{}", "PrefixExpNested");
            new PrefixExpNested_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void NestedTableTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString(@"
t = {
    f = {

    }
}");
            Assert.Equal(0, tree.ErrorList.Count);
            var generator = new TestGenerator();
            generator.GenerateTestFromString(@"
t = {
    f = {

    }
}", "NestedTable");

            new NestedTable_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void EmbeddedFunctionTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString(@"
foo = function()
    bar = function()

    end
end");
            Assert.Equal(0, tree.ErrorList.Count);
            var generator = new TestGenerator();
            generator.GenerateTestFromString(@"
foo = function()
    bar = function()

    end
end", "EmbeddedFunction");

            new EmbeddedFunction_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void IfStatementTest()
        {
            SyntaxTree tree = SyntaxTree.CreateFromString(@"
if directionIndex then
    vistedRooms = vistedRooms + 1;
else
    currentRoom = roomStack:pop();
end");
            Assert.Equal(0, tree.ErrorList.Count);
            var generator = new TestGenerator();
            generator.GenerateTestFromString(@"
if directionIndex then
    vistedRooms = vistedRooms + 1;
else
    currentRoom = roomStack:pop();
end", "IfStatement");

            new IfStatement_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void CheckForExceptionsFromListOfInvalidProgramsTest()
        {
            var reader = new StreamReader(File.OpenRead(@"CorrectSampleLuaFiles\InvalidProgramsAsStrings.lua"));
            while (!reader.EndOfStream)
            {
                char nextChar = (char)reader.Read();
                var sb = new StringBuilder();

                if (nextChar == '"')
                {
                    nextChar = (char)reader.Read();
                }

                while (nextChar != '"' && !reader.EndOfStream)
                {
                    sb.Append(nextChar);
                    nextChar = (char)reader.Read();
                }

                var tree = SyntaxTree.CreateFromString(sb.ToString());

                nextChar = (char)reader.Read();
                while (nextChar != '"' && !reader.EndOfStream)
                {
                    nextChar = (char)reader.Read();
                }
            }
        }

        [Fact]
        public void SeriesOfStringsExceptionTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\InvalidProgramsAsStrings.lua");
        }

        [Fact(Skip = "Not passing")]
        public void CheckForErrorsParsingVariousLuaFilesTest()
        {
            var generator = new TestGenerator();
            var treeList = generator.GenerateTestsForAllTestFiles();

            int fileNumber = 0;

            new Generated_0().Test(new Tester(treeList[fileNumber++]));
            new Generated_1().Test(new Tester(treeList[fileNumber++]));
            new Generated_2().Test(new Tester(treeList[fileNumber++]));
            new Generated_3().Test(new Tester(treeList[fileNumber++]));
            new Generated_4().Test(new Tester(treeList[fileNumber++]));
            new Generated_5().Test(new Tester(treeList[fileNumber++]));
            new Generated_6().Test(new Tester(treeList[fileNumber++]));
            new Generated_7().Test(new Tester(treeList[fileNumber++]));
            new Generated_8().Test(new Tester(treeList[fileNumber++]));
            new Generated_9().Test(new Tester(treeList[fileNumber++]));
            new Generated_10().Test(new Tester(treeList[fileNumber++]));
            new Generated_11().Test(new Tester(treeList[fileNumber++]));
            new Generated_12().Test(new Tester(treeList[fileNumber++]));
            new Generated_13().Test(new Tester(treeList[fileNumber++]));
            new Generated_14().Test(new Tester(treeList[fileNumber++]));
            new Generated_15().Test(new Tester(treeList[fileNumber++]));
            new Generated_16().Test(new Tester(treeList[fileNumber++]));
            new Generated_17().Test(new Tester(treeList[fileNumber++]));
            new Generated_18().Test(new Tester(treeList[fileNumber++]));
            new Generated_19().Test(new Tester(treeList[fileNumber++]));
            new Generated_20().Test(new Tester(treeList[fileNumber++]));
            new Generated_21().Test(new Tester(treeList[fileNumber++]));
            new Generated_22().Test(new Tester(treeList[fileNumber++]));
            new Generated_23().Test(new Tester(treeList[fileNumber++]));
            new Generated_24().Test(new Tester(treeList[fileNumber++]));
            new Generated_25().Test(new Tester(treeList[fileNumber++]));
            new Generated_26().Test(new Tester(treeList[fileNumber++]));
            new Generated_27().Test(new Tester(treeList[fileNumber++]));
            new Generated_28().Test(new Tester(treeList[fileNumber++]));
            new Generated_29().Test(new Tester(treeList[fileNumber++]));
            new Generated_31().Test(new Tester(treeList[fileNumber++]));
            new Generated_32().Test(new Tester(treeList[fileNumber++]));
            new Generated_33().Test(new Tester(treeList[fileNumber++]));
        }
    }
}
