using Microsoft.VisualStudio.TestTools.UnitTesting;
using LanguageService.LanguageModel.TreeVisitors;
using System.Diagnostics;
using Xunit;
using Assert = Xunit.Assert;
using LanguageModel.Tests.GeneratedTestFiles;
using LanguageModel.Tests.TestGeneration;
using LanguageModel.Tests;
using System.IO;

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
            var generator = new TestGenerator();
            generator.GenerateTestForFile(@"CorrectSampleLuaFiles\WhileStatement.lua", "WhileStatement");
            new WhileStatement_Generated().Test(new Tester(tree));
        }

        [Fact]
        public void ComplexTableConstructorGeneratedTest()
        {
            SyntaxTree tree = SyntaxTree.Create(@"CorrectSampleLuaFiles\ComplexTableConstructor.lua");
            var generator = new TestGenerator();
            generator.GenerateTestForFile(@"CorrectSampleLuaFiles\ComplexTableConstructor.lua", "ComplexTableConstructor");
            //new ComplexTableConstructor_Generated().Test(new Tester(tree));
        }

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

            foreach(var node in tree.Next(tree.Root))
            {
                if(node is Token)
                {
                    Debug.WriteLine(((Token)node).Kind.ToString() + "\n");
                } else
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
