using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.LanguageModel.TreeVisitors
{
    public class ToStringVisitor : INodeVisitor
    {
        public IndentingTextWriter IndentingWriter { get; }

        public ToStringVisitor()
        {
            IndentingWriter = IndentingTextWriter.Get(new StringWriter());
        }

        public string SyntaxTreeAsString()
        {
            return IndentingWriter.ToString();
        }

        internal override void Visit(BlockNode node)
        {
            IndentingWriter.WriteLine("Block");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        public override void Visit(ChunkNode node)
        {
            IndentingWriter.WriteLine("ChunkNode");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(Token token)
        {
            IndentingWriter.WriteLine(token.ToString());
        }

        #region Statement Node
        internal override void Visit(SemiColonStatementNode node)
        {
            IndentingWriter.WriteLine("Semi Colon Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FunctionCallStatementNode node)
        {
            IndentingWriter.WriteLine("Function Call Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ReturnStatementNode node)
        {
            IndentingWriter.WriteLine("Return Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(BreakStatementNode node)
        {
            IndentingWriter.WriteLine("Break Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(GoToStatementNode node)
        {
            IndentingWriter.WriteLine("GoToStatement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(DoStatementNode node)
        {
            IndentingWriter.WriteLine("Do Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(WhileStatementNode node)
        {
            IndentingWriter.WriteLine("While Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(RepeatStatementNode node)
        {
            IndentingWriter.WriteLine("Repeat Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(GlobalFunctionStatementNode node)
        {
            IndentingWriter.WriteLine("Function Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LocalAssignmentStatementNode node)
        {
            IndentingWriter.WriteLine("Local Asignment");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LocalFunctionStatementNode node)
        {
            IndentingWriter.WriteLine("Local Function Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(SimpleForStatementNode node)
        {
            IndentingWriter.WriteLine("For Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(MultipleArgForStatementNode node)
        {
            IndentingWriter.WriteLine("For Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(LabelStatementNode node)
        {
            IndentingWriter.WriteLine("Label Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(AssignmentStatementNode node)
        {
            IndentingWriter.WriteLine("Assignment Statement");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }
        #endregion

        #region IfStatement

        internal override void Visit(IfStatementNode node)
        {
            IndentingWriter.WriteLine("If Node");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ElseBlockNode node)
        {
            IndentingWriter.WriteLine("Else Block");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ElseIfBlockNode node)
        {
            IndentingWriter.WriteLine("ElseIf Block: ");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region Expression Nodes

        internal override void Visit(SimpleExpression node)
        {
            IndentingWriter.WriteLine("Expression");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(BinaryOperatorExpression node)
        {
            IndentingWriter.WriteLine("Expression");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(UnaryOperatorExpression node)
        {
            IndentingWriter.WriteLine("Expression");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(TableConstructorExp node)
        {
            IndentingWriter.WriteLine("Expression");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FunctionDef node)
        {
            IndentingWriter.WriteLine("FunctionDef");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region Args Nodes

        internal override void Visit(ParenArg node)
        {
            IndentingWriter.WriteLine("Args");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(TableContructorArg node)
        {
            IndentingWriter.WriteLine("Args");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(StringArg node)
        {
            IndentingWriter.WriteLine("Args");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        #region List Nodes

        internal override void Visit(NameList node)
        {
            IndentingWriter.WriteLine("Name List");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(VarList node)
        {
            IndentingWriter.WriteLine("Var List");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FieldList node)
        {
            IndentingWriter.WriteLine("Field List");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(ExpList node)
        {
            IndentingWriter.WriteLine("Expression List");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(VarArgPar node)
        {
            IndentingWriter.WriteLine("VarArg Parameter");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(NameListPar node)
        {
            IndentingWriter.WriteLine("Parameter List");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        #endregion

        internal override void Visit(TableConstructorNode node)
        {
            IndentingWriter.WriteLine("Table Constructor");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FuncBodyNode node)
        {
            IndentingWriter.WriteLine("FuncBody");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }

        internal override void Visit(FuncNameNode node)
        {
            IndentingWriter.WriteLine("FuncName");
            using (IndentingWriter.Indent())
            {
                base.Visit(node);
            }
        }
    }
}
