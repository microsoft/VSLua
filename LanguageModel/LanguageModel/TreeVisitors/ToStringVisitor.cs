using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.LanguageModel.TreeVisitors
{
    class ToStringVisitor : INodeVisitor
    {
        public IndentingTextWriter indentingWriter;

        public ToStringVisitor()
        {
            indentingWriter = IndentingTextWriter.Get(new StringWriter());
        }

        //internal override void Visit(BlockNode node)
        //{
        //    indentingWriter.WriteLine("Block");
        //    foreach (var child in node.Statements)
        //    {
        //        using (indentingWriter.Indent())
        //        {
        //            Visit(child);
        //        }
        //    }

        //    if (node.ReturnStatement != null)
        //    {
        //        using (indentingWriter.Indent())
        //        {
        //            Visit(node.ReturnStatement);
        //        }
        //    }
        //}

        //public override void Visit(ChunkNode node)
        //{
        //    indentingWriter.WriteLine("ChunkNode");
        //    using (indentingWriter.Indent())
        //    {
        //        Visit(node.ProgramBlock);
        //        Visit(node.EndOfFile);
        //    }
        //}

        //internal override void Visit(Token token)
        //{
        //    indentingWriter.WriteLine(token.ToString());
        //}

        internal override void Visit(BlockNode node)
        {
            indentingWriter.WriteLine("Block");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        public override void Visit(ChunkNode node)
        {
            indentingWriter.WriteLine("ChunkNode");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(Token token)
        {
            indentingWriter.WriteLine(token.ToString());
        }

        #region Statement Node
        internal override void Visit(SemiColonStatementNode node)
        {
            indentingWriter.WriteLine("Semi Colon Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FunctionCallStatementNode node)
        {
            indentingWriter.WriteLine("Function Call Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ReturnStatementNode node)
        {
            indentingWriter.WriteLine("Return Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(BreakStatementNode node)
        {
            indentingWriter.WriteLine("Break Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(GoToStatementNode node)
        {
            indentingWriter.WriteLine("GoToStatement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(DoStatementNode node)
        {
            indentingWriter.WriteLine("Do Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(WhileStatementNode node)
        {
            indentingWriter.WriteLine("While Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(RepeatStatementNode node)
        {
            indentingWriter.WriteLine("Repeat Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(GlobalFunctionStatementNode node)
        {
            indentingWriter.WriteLine("Function Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LocalAssignmentStatementNode node)
        {
            indentingWriter.WriteLine("Local Asignment");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LocalFunctionStatementNode node)
        {
            indentingWriter.WriteLine("Local Function Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(SimpleForStatementNode node)
        {
            indentingWriter.WriteLine("For Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(MultipleArgForStatementNode node)
        {
            indentingWriter.WriteLine("For Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LabelStatementNode node)
        {
            indentingWriter.WriteLine("Label Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(AssignmentStatementNode node)
        {
            indentingWriter.WriteLine("Assignment Statement");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }
        #endregion

        #region IfStatement

        internal override void Visit(IfStatementNode node)
        {
            indentingWriter.WriteLine("If Node");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ElseBlockNode node)
        {
            indentingWriter.WriteLine("Else Block");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ElseIfBlockNode node)
        {
            indentingWriter.WriteLine("ElseIf Block: ");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region Expression Nodes

        internal override void Visit(SimpleExpression node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(BinaryOperatorExpression node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(UnaryOperatorExpression node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(TableConstructorExp node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FunctionDef node)
        {
            indentingWriter.WriteLine("FunctionDef");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region Args Nodes

        internal override void Visit(ParenArg node)
        {
            indentingWriter.WriteLine("Args");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(TableContructorArg node)
        {
            indentingWriter.WriteLine("Args");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(StringArg node)
        {
            indentingWriter.WriteLine("Args");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region List Nodes

        internal override void Visit(NameList node)
        {
            indentingWriter.WriteLine("Name List");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(VarList node)
        {
            indentingWriter.WriteLine("Var List");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FieldList node)
        {
            indentingWriter.WriteLine("Field List");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ExpList node)
        {
            indentingWriter.WriteLine("Expression List");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(VarArgPar node)
        {
            indentingWriter.WriteLine("VarArg Parameter");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(NameListPar node)
        {
            indentingWriter.WriteLine("Parameter List");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        internal override void Visit(TableConstructorNode node)
        {
            indentingWriter.WriteLine("Table Constructor");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FuncBodyNode node)
        {
            indentingWriter.WriteLine("FuncBody");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FuncNameNode node)
        {
            indentingWriter.WriteLine("FuncName");
            using (indentingWriter.Indent())
            {
                base.Visit(node);
            }
        }
    }
}
