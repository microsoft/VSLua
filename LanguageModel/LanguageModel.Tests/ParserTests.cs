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

        //"-hello_world"

        //"+-*/"

        //"x ==--[[ comment ]]y"

        //"x..y"

        //"x +1 == 2   x= 3 /2+4"

        //"
        //x=
        //1"

        //"1     +       2        =       x"

        //"
        //1+1+
        //1"

        //"
        //1+1+
        //1"

        //"1+"

        //"t[1]"

        //"}("

        //")("

        //"]("

        //"t = [ 1--[[ comment ]]]"

        //"foo = function (x, y, z, w) end"

        //"t[\"this is a test that grabs this key in Lua\"]"

        //"t = {1, 3, 4, 5, 6, 7,}"

        //"
        //t = {
        //    1,
        //    2,
        //}"

        //"
        //foo = function(
        //    a, b, c)"

        //"
        //t = [


        //1


        //]"

        //"
        //t :
        //foo()
        //t.
        // bar ()"

        //"
        //foo = function()
        //    return
        //end"

        //"
        //foo = function()
        //return
        //end"

        //"
        //t1 = {
        //1,
        //}"

        //"
        //foo = function
        //--[[comment]] return
        //end"

        //"
        //foo = function
        //bar = function
        //end
        //end"

        //"
        //t1 = {
        //t2 = {
        //x, y, z
        //    }
        //    t3 = {
        //x
        //}
        //}"

        //"
        //foo = function()
        //return--comment
        //    end"

        //"     foo"

        //"

        //foo = function()
        //                  return
        //end"

        //"{
        //       x"

        //",x"

        //"x,= 1"

        //"{ x,,y }"

        //",--[[ comment ]]x"

        //"{ x,y,z }"

        //"{ x,y, ,,,, },y32,,,s2,"

        //"x,             y"

        //"
        //x,
        //y = 1,2"

        //"x,y = 1,2"

        //"function foo(x,y,z)"

        //"{ x, y,}"

        //"x,y"

        //"
        //x

        //x       "

        //"
        //x = 10           
        //-- comment here
        //--[[block
        //comment
        //here]]
        //x = x + 1            "

        //"x   
        //    "




        //[Fact]
        //public void FunctionDefErrorGeneratedTest()
        //{
        //    SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\FunctionDefError.lua");
        //    var generator = new TestGenerator();
        //    generator.GenerateTestForFile(@"CorrectSampleLuaFiles\FunctionDefError.lua", "FunctionDefError");
        //    new FunctionDefError_Generated().Test(new Tester(tree));
        //}

        //[Fact]
        //public void AutoGenerateTests()
        //{
        //    var generator = new TestGenerator();
        //    generator.GenerateTestsForAllFiles();
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
