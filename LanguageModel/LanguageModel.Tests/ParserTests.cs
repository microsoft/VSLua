using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanguageService.LanguageModel.TreeVisitors;
using System.Diagnostics;
using Xunit;
using Assert = Xunit.Assert;
using LanguageModel.Tests.GeneratedTestFiles;
using LanguageModel.Tests.TestGeneration;
using LanguageModel.Tests;
using System.IO;
using System.Text;

namespace LanguageService.Tests
{
    [DeploymentItem("CorrectSampleLuaFiles", "CorrectSampleLuaFiles")]
    [DeploymentItem("SerializedJsonOutput", "SerializedJsonOutput")]
    public class ParserTests
    {
        [Fact]
        public void SmallIfGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\smallif.lua");
            new SmallIf_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void AssignmentsGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\Assignments.lua");
            new Assignments_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void MultipleTypeAssignmentGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\MultipleTypeAssignment.lua");
            new MultipleTypeAssignment_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void WhileStatementGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\WhileStatement.lua");
            new WhileStatement_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void ComplexTableConstructorGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\ComplexTableConstructor.lua");
            new ComplexTableConstructor_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void TableStatementsGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\TableStatements.lua");
            new TableStatements_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void TripleNestedFunctionCallGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\TripleNestedFunctionCall.lua"); ;
            new TripleNestedFunctionCall_Generated().Test(new Tester(tree));
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
        }
        
        [Fact]
        public void CheckForExceptionsFromListOfInvalidProgramsTest()
        {
            var reader = new StreamReader(File.OpenRead(@"CorrectSampleLuaFiles\InvalidProgramsAsStrings.lua"));
            while (!reader.EndOfStream)
            {
                char nextChar = (char)reader.Read();
                var sb = new StringBuilder();

                if(nextChar == '"')
                    nextChar = (char)reader.Read();

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
        public void SeriesOfStringsErrorTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\InvalidProgramsAsStrings.lua");
        }

        [Fact]
        public void CheckForErrorsParsingVariousLuaFilesTest()
        {
            var generator = new TestGenerator();
            generator.GenerateTestsForAllFiles();
        }

        //[Fact]
        //public void SimpleTableGeneratedTest()
        //{
        //    SyntaxTree tree = SyntaxTree.CreateFromString("1 +1");
        //    var generator = new TestGenerator();
        //    generator.GenerateTestFromString(@"CorrectSampleLuaFiles\FunctionDefError.lua", "FunctionDefError");
        //    new SimpleTableError_Generated().Test(new Tester(tree));
        //}

        //[Fact]
        //public void FunctionDefErrorGeneratedTest()
        //{
        //    SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\FunctionDefError.lua");
        //    var generator = new TestGenerator();
        //    generator.GenerateTestForFile(@"CorrectSampleLuaFiles\FunctionDefError.lua", "FunctionDefError");
        //    new FunctionDefError_Generated().Test(new Tester(tree));
        //}

        [Fact]
        public void DebugTreeEnumeratorMethod()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\smallif.lua");

            var visitor = new StringWalker();
            tree.Root.Accept(visitor);

            Debug.WriteLine(visitor.SyntaxTreeAsString());

            foreach (var node in tree.Next(tree.Root))
            {
                if (node is Token)
                {
                    Debug.WriteLine(((Token)node).Kind.ToString() + "\n");
                }
                else
                {
                    Debug.WriteLine(((SyntaxNode)node).Kind.ToString() + "\n");
                }

            }

            foreach (var error in tree.ErrorList)
                Debug.WriteLine(error.Message);

            Assert.Equal(0, tree.ErrorList.Count);
        }
    }
}
