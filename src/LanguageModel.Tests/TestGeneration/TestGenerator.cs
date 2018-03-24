// Copyright (c) Microsoft. All rights reserved.

namespace LanguageModel.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using LanguageService;
    using Xunit;

    public class TestGenerator
    {
        internal IndentingTextWriter IndentingWriter { get; private set; }
        public StringBuilder sb { get; private set; }

        private static readonly string BasePath = Environment.ExpandEnvironmentVariables(@"%UserProfile%\\Documents\\LuaTests");
        private static readonly string GenPath = Path.Combine(BasePath, "Generated Test Files");
        private static readonly string GenFileFormat = "{0}_Generated.cs";
        private static readonly string GenFileName = "Generated_{0}";

        private string GetGenFilePath(string fileName)
        {
            return Path.Combine(GenPath, string.Format(GenFileFormat, fileName));
        }

        public List<SyntaxTree> GenerateTestsForAllTestFiles()
        {
            if (!Directory.Exists(GenPath))
            {
                Directory.CreateDirectory(GenPath);
            }

            var treeList = new List<SyntaxTree>();

            int fileNumber = 0;

            foreach (string file in Directory.EnumerateFiles(Path.Combine(BasePath, "Lua Files for Testing"), "*.lua"))
            {
                SyntaxTree tree = SyntaxTree.Create(file);
                File.WriteAllText(this.GetGenFilePath(fileNumber.ToString()), string.Format(@"//{0}{1}", file, this.GenerateTest(tree, string.Format(GenFileName, fileNumber.ToString()))));

                Assert.Equal(0, tree.ErrorList.Count);

                treeList.Add(tree);

                fileNumber++;
            }

            return treeList;
        }

        public void GenerateTestFromFile(string filePath, string name)
        {
            SyntaxTree tree = SyntaxTree.Create(filePath);
            File.WriteAllText(this.GetGenFilePath(name), this.GenerateTest(tree, name + "_Generated"));
        }

        public void GenerateTestFromString(string program, string name)
        {
            SyntaxTree tree = SyntaxTree.CreateFromString(program);
            File.WriteAllText(this.GetGenFilePath(name), this.GenerateTest(tree, name + "_Generated"));
        }

        public string GenerateTest(SyntaxTree tree, string name)
        {
            this.IndentingWriter = IndentingTextWriter.Get(new StringWriter());
            this.sb = new StringBuilder();

            this.sb.AppendLine();
            this.sb.AppendLine("using LanguageModel.Tests.TestGeneration;");
            this.sb.AppendLine("using LanguageService;");
            this.sb.AppendLine("using Xunit;");
            this.sb.AppendLine("namespace LanguageModel.Tests.GeneratedTestFiles");
            this.sb.AppendLine("{");
            this.sb.AppendLine(string.Format("    class {0}", name));
            this.sb.AppendLine("    {");
            this.sb.AppendLine("        [Fact]");
            this.sb.AppendLine("        public void Test(Tester t)");
            this.sb.AppendLine("        {");

            using (this.IndentingWriter.Indent())
            {
                using (this.IndentingWriter.Indent())
                {
                    using (this.IndentingWriter.Indent())
                    {
                        this.GenerateTestStructure(tree.Root);
                    }
                }
            }

            this.sb.Append(this.IndentingWriter.ToString());
            this.sb.AppendLine("        }");
            this.sb.AppendLine("    }");
            this.sb.AppendLine("}");
            return this.sb.ToString();
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
                this.IndentingWriter.WriteLine("t.N(SyntaxKind." + ((SyntaxNode)syntaxNodeOrToken).Kind + ");");
            }
            else
            {
                this.IndentingWriter.WriteLine("t.N(SyntaxKind." + ((Token)syntaxNodeOrToken).Kind + ");");
            }

            if (!syntaxNodeOrToken.IsLeafNode)
            {
                this.IndentingWriter.WriteLine("{");
                foreach (var node in ((SyntaxNode)syntaxNodeOrToken).Children)
                {
                    using (this.IndentingWriter.Indent())
                    {
                        this.GenerateTestStructure(node);
                    }
                }
                this.IndentingWriter.WriteLine("}");
            }
        }
    }
}
