using LanguageService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LanguageModel.Tests
{
    class TestGenerator
    {
        public IndentingTextWriter IndentingWriter { get; private set; }
        public StringBuilder SB { get; private set; }

        public void GenerateTestsForAllFiles()
        {
            int fileNumber = 0;
            //TODO avoid hardcoding file path
            foreach (string file in Directory.EnumerateFiles(@"C:\\Users\\t-kevimi\\Documents\\Engineering\\Lua Files for Testing", "*.lua"))
            {
                SyntaxTree tree = SyntaxTree.Create(file);
                File.WriteAllText("C:\\Users\\t-kevimi\\Documents\\Engineering\\Generated Test Files\\" + fileNumber + "_Generated.cs", @"//" + file + "\n" + GenerateTest(tree));

                foreach (var error in tree.ErrorList)
                    Debug.WriteLine(error.Message);

                fileNumber++;
            }
        }

        public void GenerateTestForFile(string filePath, string name)
        {
            SyntaxTree tree = SyntaxTree.Create(filePath);
            File.WriteAllText("C:\\Users\\t-kevimi\\Documents\\Engineering\\Generated Test Files\\" + name + "_Generated.cs", GenerateTest(tree));
        }

        public string GenerateTest(SyntaxTree tree)
        {
            //ISSUE: bad practice?
            IndentingWriter = IndentingTextWriter.Get(new StringWriter());
            SB = new StringBuilder();

            SB.Append(@"using LanguageModel.Tests.TestGeneration;
using LanguageService;
using Xunit;

namespace LanguageModel.Tests.GeneratedTestFiles
{
    class SmallIf_Generated
    {
        [Fact]
        public void Test(Tester t)
        {
");

            using (IndentingWriter.Indent())
            {
                using (IndentingWriter.Indent())
                {
                    using (IndentingWriter.Indent())
                    {
                        GenerateTestStructure(tree.Root);
                    };
                };
            };

            SB.Append(IndentingWriter.ToString());
            SB.Append(@"
        }
    }
}");
            return SB.ToString();
        }

        private void GenerateTestStructure(SyntaxNodeOrToken syntaxNodeOrToken)
        {

            if (syntaxNodeOrToken == null)
            {
                return;
            }

            //TODO remove is-check once Immutable graph object bug is fixed. 
            if (syntaxNodeOrToken is SyntaxNode)
            {
                IndentingWriter.WriteLine("t.N(SyntaxKind." + ((SyntaxNode)syntaxNodeOrToken).Kind + ");");
            }
            else
            {
                IndentingWriter.WriteLine("t.N(SyntaxKind." + ((Token)syntaxNodeOrToken).Kind + ");");
            }

            if (!SyntaxTree.IsLeafNode(syntaxNodeOrToken))
            {
                IndentingWriter.WriteLine("{");
                foreach (var node in ((SyntaxNode)syntaxNodeOrToken).Children)
                {
                    using (IndentingWriter.Indent())
                    {
                        GenerateTestStructure(node);
                    }
                }
                IndentingWriter.WriteLine("}");
            }
        }
    }
}
