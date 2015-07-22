using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.LanguageModel.TreeVisitors
{
    public interface INodeVisitor
    {
        void Visit(Token token);
        void Visit(ChunkNode node);
        void Visit(BlockNode node);
        void Visit(StatementNode node);
        void Visit(ExpressionNode node);
        void Visit(IfStatementNode node);
        void Visit(SimpleExpression node);
        void Visit(BinaryOperatorExpression node);
        void Visit(UnaryOperatorExpression node);
        void Visit(TableConstructorExp node);
        void Visit(FunctionDef node);
        void Visit(ElseIfBlockNode node);
        void Visit(ElseBlockNode node);
        void Visit(ReturnStatementNode node);
        void Visit(FieldNode node);
        void Visit(FunctionCallExp node);
        void Visit(ParenPrefixExp node);
        void Visit(NameVar node);
        void Visit(SquareBracketVar node);
        void Visit(DotVar node);
        void Visit(FieldList node);
        void Visit(NameList node);
        void Visit(ExpList node);
        void Visit(ParList node);
        void Visit(SemiColonStatementNode node);
        void Visit(BracketField node);
        void Visit(SimpleField node);
        void Visit(ExpField node);
        void Visit(TableConstructorNode node);
        void Visit(FuncBodyNode node);
        void Visit(StringArg stringArg);
        void Visit(BreakStatementNode breakStatementNode);
        void Visit(FuncNameNode funcNameNode);
        void Visit(NameDotPair nameDotPair);
    }
}

#region temporary visit implementation boilerplatecode placeholder(to be removed)

//    class ConcreteVisitor : INodeVisitor
//    {

//        public void Visit(SimpleExpression node)
//        {
//            Visit(node.ExpressionValue);
//        }

//        public void Visit(UnaryOperatorExpression node)
//        {
//            Visit(node.UnaryOperator);
//            Visit(node.Exp);
//        }

//        public void Visit(ElseIfBlockNode node)
//        {

//            Visit(node.Exp);
//            Visit(node.Block);
//        }

//        public void Visit(TableConstructorExp node)
//        {
//            Visit(node.FieldList);
//        }

//        public void Visit(BinaryOperatorExpression node)
//        {
//            Visit(node.Exp1);
//            Visit(node.BinaryOperator);
//            Visit(node.Exp2);
//        }

//        public void Visit(IfStatementNode node)
//        {
//            Visit(node.Exp);
//            Visit(node.IfBlock);

//            if (node.ElseIfList != null)
//            {
//                foreach (var block in node.ElseIfList)
//                {
//                    if (block != null)
//                    {
//                        Visit(block);
//                    }
//                    else
//                    {
//                        //"null"); //TODO: validate will ever reach here?
//                    }
//                }
//            }

//            if (node.ElseBlock != null)
//            {
//                Visit(node.ElseBlock);
//            }
//        }

//        public void Visit(BlockNode node)
//        {
//            foreach (var child in node.Statements)
//            {
//                Visit(child);
//            }

//            if (node.ReturnStatement != null)
//            {
//                Visit(node.ReturnStatement);
//            }
//        }

//        public void Visit(ChunkNode node)
//        {
//            Visit(node.ProgramBlock);
//            Visit(node.EndOfFile);
//        }

//        public void Visit(Token token)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(MisplacedTokenNode node)
//        {
//            Visit(node.Token);
//        }

//        public void Visit(StatementNode node)
//        {
//            //TODO remove after all statements implemented
//            bool isStatement = false;
//            if (node as IfStatementNode != null)
//            {
//                Visit(node as IfStatementNode);
//                isStatement = true;
//            }
//            if (node as MisplacedTokenNode != null)
//            {
//                Visit(node as MisplacedTokenNode);
//                isStatement = true;
//            }
//            if (node as SemiColonStatementNode != null)
//            {
//                Visit(node as SemiColonStatementNode);
//                isStatement = true;
//            }
//            if (!isStatement)
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public void Visit(ElseBlockNode node)
//        {
//            Visit(node.Block);
//        }

//        #region Not Implemented Visit Methods
//        public void Visit(ExpressionNode node)
//        {
//            if (node as SimpleExpression != null)
//                Visit(node as SimpleExpression);
//            if (node as BinaryOperatorExpression != null)
//                Visit(node as BinaryOperatorExpression);
//            if (node as UnaryOperatorExpression != null)
//                Visit(node as UnaryOperatorExpression);
//            if (node as TableConstructorExp != null)
//                Visit(node as TableConstructorExp);
//            if (node as FunctionDef != null)
//                Visit(node as FunctionDef);
//            if (node as PrefixExp != null)
//                Visit(node as PrefixExp);
//        }

//        public void Visit(SemiColonStatementNode node)
//        {
//            Visit(node.SemiColon);
//        }

//        public void Visit(ExpList node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(SimpleField node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(TableConstructorNode node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(FuncBodyNode node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(ExpField node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(BracketField node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(ParList node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(NameList node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(DotVar node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(NameVar node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(FunctionCallExp node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(ReturnStatementNode node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(FunctionDef node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(FieldNode node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(ParenPrefixExp node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(SquareBracketVar node)
//        {
//            throw new NotImplementedException();
//        }

//        public void Visit(FieldList node)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}

#endregion