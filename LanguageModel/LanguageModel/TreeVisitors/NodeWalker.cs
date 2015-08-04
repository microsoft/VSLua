using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.LanguageModel.TreeVisitors
{
    public abstract class NodeWalker
    {
        public virtual void Visit(ChunkNode node)
        {
            Visit(node.ProgramBlock);
            Visit(node.EndOfFile);
        }

        internal abstract void Visit(Token token);

        internal virtual void Visit(BlockNode node)
        {
            foreach (var child in node.Statements)
            {
                Visit(child);
            }
        }

        internal virtual void Visit(StatementNode node)
        {
            if (node is IfStatementNode)
            {
                Visit(node as IfStatementNode);
            }
            else if (node is SemiColonStatementNode)
            {
                Visit(node as SemiColonStatementNode);
            }
            else if (node is FunctionCallStatementNode)
            {
                Visit(node as FunctionCallStatementNode);
            }
            else if (node is LocalAssignmentStatementNode)
            {
                Visit(node as LocalAssignmentStatementNode);
            }
            else if (node is LabelStatementNode)
            {
                Visit(node as LabelStatementNode);
            }
            else if (node is BreakStatementNode)
            {
                Visit(node as BreakStatementNode);
            }
            else if (node is GoToStatementNode)
            {
                Visit(node as GoToStatementNode);
            }
            else if (node is DoStatementNode)
            {
                Visit(node as DoStatementNode);
            }
            else if (node is WhileStatementNode)
            {
                Visit(node as WhileStatementNode);
            }
            else if (node is RepeatStatementNode)
            {
                Visit(node as RepeatStatementNode);
            }
            else if (node is MultipleArgForStatementNode)
            {
                Visit(node as MultipleArgForStatementNode);
            }
            else if (node is SimpleForStatementNode)
            {
                Visit(node as SimpleForStatementNode);
            }
            else if (node is LocalAssignmentStatementNode)
            {
                Visit(node as LocalAssignmentStatementNode);
            }
            else if (node is LocalFunctionStatementNode)
            {
                Visit(node as LocalFunctionStatementNode);
            }
            else if (node is AssignmentStatementNode)
            {
                Visit(node as AssignmentStatementNode);
            }
            else
            {
                throw new ArgumentException();
            }

        }

        #region Simple Statement Nodes
        internal virtual void Visit(SemiColonStatementNode node)
        {
            Visit(node.SemiColon);
        }

        internal virtual void Visit(FunctionCallStatementNode node)
        {
            Visit(node.PrefixExp);
            if (node.Colon != null)
                Visit(node.Colon);
            if (node.Name != null)
                Visit(node.Name);
            Visit(node.Args);
        }

        internal virtual void Visit(ReturnStatementNode node)
        {
            Visit(node.ReturnKeyword);
            Visit(node.ExpList);
            Visit(node.SemiColon);
        }

        internal virtual void Visit(BreakStatementNode node)
        {
            Visit(node.BreakKeyword);
        }

        internal virtual void Visit(GoToStatementNode node)
        {
            Visit(node.Name);
        }

        internal virtual void Visit(DoStatementNode node)
        {
            Visit(node.Block);
        }

        internal virtual void Visit(WhileStatementNode node)
        {
            Visit(node.Exp);
            Visit(node.Block);
        }

        internal virtual void Visit(RepeatStatementNode node)
        {
            Visit(node.Block);
            Visit(node.Exp);
        }

        internal virtual void Visit(GlobalFunctionStatementNode node)
        {
            Visit(node.FuncName);
            Visit(node.FuncBody);
        }

        internal virtual void Visit(LocalAssignmentStatementNode node)
        {
            Visit(node.LocalKeyword);
            Visit(node.NameList);
            Visit(node.ExpList);
        }

        internal virtual void Visit(LocalFunctionStatementNode node)
        {
            Visit(node.LocalKeyword);
            Visit(node.Name);
            Visit(node.FuncBody);
        }

        internal virtual void Visit(SimpleForStatementNode node)
        {
            Visit(node.Name);
            Visit(node.Exp1);
            Visit(node.Exp2);
            if (node.OptionalComma != null)
                Visit(node.OptionalComma);
            if (node.OptionalExp3 != null)
                Visit(node.OptionalExp3);
            Visit(node.Block);
        }

        internal virtual void Visit(MultipleArgForStatementNode node)
        {
            Visit(node.NameList);
            Visit(node.ExpList);
            Visit(node.Block);
        }

        internal virtual void Visit(LabelStatementNode node)
        {
            Visit(node.Name);
        }

        internal virtual void Visit(AssignmentStatementNode node)
        {
            Visit(node.VarList);
            Visit(node.AssignmentOperator);
            Visit(node.ExpList);
        }

        #endregion

        #region If Statement
        internal virtual void Visit(IfStatementNode node)
        {
            Visit(node.Exp);
            Visit(node.IfBlock);

            if (node.ElseIfList != null)
            {
                foreach (var elseIfBlock in node.ElseIfList)
                {
                    Visit(elseIfBlock);
                }
            }

            if (node.ElseBlock != null)
            {
                Visit(node.ElseBlock);
            }
        }

        internal virtual void Visit(ElseIfBlockNode node)
        {
            Visit(node.Exp);
            Visit(node.Block);
        }

        internal virtual void Visit(ElseBlockNode node)
        {
            Visit(node.Block);
        }

        #endregion

        #region Expression Nodes
        internal virtual void Visit(ExpressionNode node)
        {
            if (node is SimpleExpression)
                Visit(node as SimpleExpression);
            else if (node is BinaryOperatorExpression)
                Visit(node as BinaryOperatorExpression);
            else if (node is UnaryOperatorExpression)
                Visit(node as UnaryOperatorExpression);
            else if (node is TableConstructorExp)
                Visit(node as TableConstructorExp);
            else if (node is FunctionDef)
                Visit(node as FunctionDef);
            else if (node is Var)
                Visit(node as Var);
            else if (node is FunctionCallExp)
                Visit(node as FunctionCallExp);
            else if (node is ParenPrefixExp)
                Visit(node as ParenPrefixExp);
            else
                throw new ArgumentException();
        }

        internal virtual void Visit(SimpleExpression node)
        {
            Visit(node.ExpressionValue);
        }

        internal virtual void Visit(BinaryOperatorExpression node)
        {
            Visit(node.Exp1);
            Visit(node.BinaryOperator);
            Visit(node.Exp2);
        }

        internal virtual void Visit(UnaryOperatorExpression node)
        {
            Visit(node.UnaryOperator);
            Visit(node.Exp);
        }

        internal virtual void Visit(TableConstructorExp node)
        {
            Visit(node.FieldList);
        }

        internal virtual void Visit(FunctionDef node)
        {
            Visit(node.FunctionBody);
        }

        #region FieldNode Expression
        internal virtual void Visit(FieldNode node)
        {
            if (node is ExpField)
            {
                Visit(node as ExpField);
            }
            else if (node is BracketField)
            {
                Visit(node as BracketField);
            }
            else if (node is AssignmentField)
            {
                Visit(node as AssignmentField);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal virtual void Visit(BracketField node)
        {
            Visit(node.IdentifierExp);
            Visit(node.AssignedExp);
        }

        internal virtual void Visit(AssignmentField node)
        {
            Visit(node.Name);
            Visit(node.Exp);
        }

        internal virtual void Visit(ExpField node)
        {
            Visit(node.Exp);
        }

        #endregion

        #region PrefixExp Expression
        internal virtual void Visit(Var node)
        {
            if (node is NameVar)
            {
                Visit(node as NameVar);
            }
            else if (node is SquareBracketVar)
            {
                Visit(node as SquareBracketVar);
            }
            else if (node is DotVar)
            {
                Visit(node as DotVar);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal virtual void Visit(NameVar node)
        {
            Visit(node.Name);
        }

        internal virtual void Visit(SquareBracketVar node)
        {
            Visit(node.PrefixExp);
            Visit(node.Exp);
        }

        internal virtual void Visit(DotVar node)
        {
            Visit(node.PrefixExp);
            Visit(node.DotOperator);
            Visit(node.NameIdentifier);
        }

        internal virtual void Visit(FunctionCallExp node)
        {
            Visit(node.PrefixExp);
            if (node.Colon != null)
                Visit(node.Colon);
            if (node.Name != null)
                Visit(node.Name);
            Visit(node.Args);
        }

        internal virtual void Visit(ParenPrefixExp node)
        {
            Visit(node.Exp);
        }

        #endregion

        #endregion

        #region Args Nodes
        internal virtual void Visit(Args node)
        {
            if (node is TableContructorArg)
            {
                Visit(node as TableContructorArg);
            }
            else if (node is ParenArg)
            {
                Visit(node as ParenArg);
            }
            else if (node is StringArg)
            {
                Visit(node as StringArg);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal virtual void Visit(ParenArg node)
        {
            Visit(node.ExpList);
        }

        internal virtual void Visit(TableContructorArg node)
        {
            Visit(node.FieldList);
        }

        internal virtual void Visit(StringArg node)
        {
            Visit(node.StringLiteral);
        }

        #endregion

        #region List Nodes

        internal virtual void Visit(SeparatedList node)
        {
            foreach (var listItem in node.SyntaxList)
            {
                Visit(listItem);
            }
        }

        internal virtual void Visit(SeparatedListElement node)
        {
            if (node.Element is Var)
            {
                Visit(node.Element as Var);
            }
            else if (node.Element is Token)
            {
                Visit(node.Element as Token);
            }
            else if (node.Element is ExpressionNode)
            {
                Visit(node.Element as ExpressionNode);
            }
            else if (node.Element is FieldNode)
            {
                Visit(node.Element as FieldNode);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        internal virtual void Visit(ParList node)
        {
            if (node is VarArgParList)
            {
                Visit(node as VarArgParList);
            }
            else if (node is NameListPar)
            {
                Visit(node as NameListPar);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal virtual void Visit(VarArgParList node)
        {
            Visit(node.VarargOperator);
        }

        internal virtual void Visit(NameListPar node)
        {
            Visit(node.NamesList);
            if (node.Comma != null)
            {
                Visit(node.Vararg);
            }
        }

        #endregion

        internal virtual void Visit(TableConstructorNode node)
        {
            Visit(node.FieldList);
        }

        internal virtual void Visit(FuncBodyNode node)
        {
            Visit(node.ParameterList);
            Visit(node.Block);
        }

        internal virtual void Visit(FuncNameNode node)
        {
            foreach (var seperatedElement in node.FuncNameList.SyntaxList)
            {
                throw new NotImplementedException();
                //Visit(seperatedElement.Element);
            }

            if (node.OptionalColon != null)
                Visit(node.OptionalColon);
            if (node.Name != null)
                Visit(node.Name);
        }
    }
}

