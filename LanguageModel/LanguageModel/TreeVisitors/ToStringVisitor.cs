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

        public void Visit(SimpleExpression node)
        {
            indentingWriter.WriteLine("Expression:");
            using (indentingWriter.Indent())
            {
                Visit(node.ExpressionValue);
            }
        }

        public void Visit(UnaryOperatorExpression node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                Visit(node.UnaryOperator);
                Visit(node.Exp);
            }
        }

        public void Visit(ElseIfBlockNode node)
        {
            indentingWriter.WriteLine("ElseIfBlock: ");
            using (indentingWriter.Indent())
            {
                Visit(node.Exp);
                Visit(node.Block);
            }
        }

        public void Visit(TableConstructorExp node)
        {
            indentingWriter.WriteLine("TableConstructor");
            using (indentingWriter.Indent())
            {
                Visit(node.FieldList);
            }
        }

        public void Visit(BinaryOperatorExpression node)
        {
            indentingWriter.WriteLine("Expression");
            using (indentingWriter.Indent())
            {
                Visit(node.Exp1);
                Visit(node.BinaryOperator);
                Visit(node.Exp2);
            }
        }

        public void Visit(IfStatementNode node)
        {
            indentingWriter.WriteLine("IfNode");
            using (indentingWriter.Indent())
            {
                Visit(node.Exp);
                Visit(node.IfBlock);
            }

            if (node.ElseIfList != null)
            {
                foreach (var block in node.ElseIfList)
                {
                    using (indentingWriter.Indent())
                    {
                        if (block != null)
                        {
                            Visit(block);
                        }
                        else
                        {
                            indentingWriter.WriteLine("null"); //TODO: validate will ever reach here?
                        }
                    }
                }
            }

            if (node.ElseBlock != null)
            {
                using (indentingWriter.Indent())
                {
                    Visit(node.ElseBlock);
                }
            }
        }

        public void Visit(BlockNode node)
        {
            indentingWriter.WriteLine("Block");
            foreach (var child in node.Statements)
            {
                using (indentingWriter.Indent())
                {
                    Visit(child);
                }
            }

            if (node.ReturnStatement != null)
            {
                using (indentingWriter.Indent())
                {
                    Visit(node.ReturnStatement);
                }
            }
        }

        public void Visit(ChunkNode node)
        {
            indentingWriter.WriteLine("ChunkNode");
            using (indentingWriter.Indent())
            {
                Visit(node.ProgramBlock);
                Visit(node.EndOfFile);
            }
        }

        public void Visit(Token token)
        {
            indentingWriter.WriteLine(token.ToString());
        }

        public void Visit(MisplacedTokenNode node)
        {
            indentingWriter.WriteLine("MisplacedToken");
            using (indentingWriter.Indent())
            {
                Visit(node.Token);
            }
        }

        public void Visit(StatementNode node)
        {
            //TODO remove after all statements implemented
            bool isStatement = false;
            if (node as IfStatementNode != null)
            {
                Visit(node as IfStatementNode);
                isStatement = true;
            }
            if (node as MisplacedTokenNode != null)
            {
                Visit(node as MisplacedTokenNode);
                isStatement = true;
            }
            if (node as SemiColonStatementNode != null)
            {
                Visit(node as SemiColonStatementNode);
                isStatement = true;
            }
            if (!isStatement)
            {
                throw new NotImplementedException();
            }
        }

        public void Visit(ElseBlockNode node)
        {
            indentingWriter.WriteLine("ElseBlock");
            using (indentingWriter.Indent())
            {
                Visit(node.Block);
            }
        }

        #region Not Implemented Visit Methods
        public void Visit(ExpressionNode node)
        {
            if (node as SimpleExpression != null)
                Visit(node as SimpleExpression);
            if (node as BinaryOperatorExpression != null)
                Visit(node as BinaryOperatorExpression);
            if (node as UnaryOperatorExpression != null)
                Visit(node as UnaryOperatorExpression);
            if (node as TableConstructorExp != null)
                Visit(node as TableConstructorExp);
            if (node as FunctionDef != null)
                Visit(node as FunctionDef);
            if (node as PrefixExp != null)
                Visit(node as PrefixExp);
        }

        public void Visit(ExpList node)
        {
            throw new NotImplementedException();
        }

        public void Visit(SemiColonStatementNode node)
        {
            indentingWriter.WriteLine("SemiColonStatement\t ;");
        }

        public void Visit(SimpleField node)
        {
            throw new NotImplementedException();
        }

        public void Visit(TableConstructorNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FuncBodyNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ExpField node)
        {
            throw new NotImplementedException();
        }

        public void Visit(BracketField node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ParList node)
        {
            throw new NotImplementedException();
        }

        public void Visit(NameList node)
        {
            throw new NotImplementedException();
        }

        public void Visit(DotVar node)
        {
            throw new NotImplementedException();
        }

        public void Visit(NameVar node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionCallExp node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatementNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionDef node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FieldNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ParenPrefixExp node)
        {
            throw new NotImplementedException();
        }

        public void Visit(SquareBracketVar node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FieldList node)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
