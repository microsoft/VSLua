using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Immutable;

namespace LanguageModel
{
    public static class Parser //TODO: change to streamreader so as to auto deal with encoding issues....
    {
        public static void CreateSyntaxTree(Stream stream)
        {
            IEnumerator<Token> tokenEnumerator = Lexer.Tokenize(stream).GetEnumerator();
            if (!tokenEnumerator.MoveNext())
            {
                //TODO: error?
            }
            ParseChunkNode(tokenEnumerator);
        }

        private static SyntaxNode ParseStatement(IEnumerator<Token> tokenEnumerator)
        {
            switch (tokenEnumerator.Current.Text)
            {
                case "break":
                    //TODO: Implement
                    break;
                case "goto":
                    //TODO: Implement
                    break;
                case "do":
                    //TODO: Implement
                    break;
                case "while":
                    //TODO: Implement
                    break;
                case "repeat":
                    //TODO: Implement
                    break;
                case "if":
                    return ParseIfNode(tokenEnumerator);
                case "for":
                    //TODO: Implement
                    break;
                case "function":
                    //TODO: Implement
                    break;
                case "local":
                    //TODO: Implement
                    break;
                default:
                    //TODO: Implement
                    //not the beginning of a statement?
                    break;
            }
            return null;//TODO: remove this
        }

        private static IfNode ParseIfNode(IEnumerator<Token> tokenEnumerator)
        {
            IfNode.Builder ifNodeBuilder = IfNode.CreateBuilder();
            ifNodeBuilder.ExtractTokenInfoWithTrivia(tokenEnumerator.Current);

            ifNodeBuilder.IfKeyword = Keyword.CreateBuilder();
            ifNodeBuilder.IfKeyword.ExtractKeywordInfo(tokenEnumerator.Current);

            if(!tokenEnumerator.MoveNext())
            {
                //TODO: Error?
            }

            ifNodeBuilder.Exp = Expression.CreateBuilder();
            ifNodeBuilder.Exp = ParseExp(tokenEnumerator, TokenType.ThenKeyword);

            if (tokenEnumerator.Current.Text == "then")
            {
                ifNodeBuilder.ThenKeyword = Keyword.CreateBuilder();
                ifNodeBuilder.ThenKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
            } else
            {
                //TODO: error?
            }

            if (!tokenEnumerator.MoveNext())
            {
                //TODO: Error?
            }

            ifNodeBuilder.IfBlock = Block.CreateBuilder();
            ifNodeBuilder.IfBlock = ParseIfBlock(tokenEnumerator);

            while(tokenEnumerator.Current.Type == TokenType.ElseIfKeyword)
            {
                ifNodeBuilder.ElseIfList.Add(ParseElseIfBlocks(tokenEnumerator));
            }

            if(tokenEnumerator.Current.Type == TokenType.ElseKeyword)
            {
                ifNodeBuilder.ElseBlock.ElseKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
                ifNodeBuilder.ElseBlock.Block = ParseBlock(tokenEnumerator, TokenType.EndKeyword);
            }

            if(tokenEnumerator.Current.Type != TokenType.EndKeyword)
            {
                ifNodeBuilder.EndKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
            } else
            {
                //TODO: error
            }
            
            ifNodeBuilder.Length = tokenEnumerator.Current.FullStart - ifNodeBuilder.FullStartPosition;
            
            return ifNodeBuilder.ToImmutable();

        }

        private static ElseIfBlock ParseElseIfBlocks(IEnumerator<Token> tokenEnumerator)
        {
            ElseIfBlock.Builder elseifBlockBuilder = ElseIfBlock.CreateBuilder();
            //elseifBlockBuilder.ExtractTokenInfo(tokenEnumerator.Current);

            elseifBlockBuilder.ElseIfKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
            if (!tokenEnumerator.MoveNext())
            {
                //TODO: Error?
            }

            elseifBlockBuilder.Exp = ParseExp(tokenEnumerator, TokenType.ThenKeyword);

            elseifBlockBuilder.ThenKeyword.ExtractKeywordInfo(tokenEnumerator.Current);
            if (!tokenEnumerator.MoveNext())
            {
                //TODO: Error?
            }

            elseifBlockBuilder.Block = ParseIfBlock(tokenEnumerator);

            //TODO: elseifBlockBuilder.Length = tokenEnumerator.Current.FullStart - elseifBlockBuilder.FullStartPosition;

            return elseifBlockBuilder.ToImmutable();
        }

        private static Expression.Builder ParseExp(IEnumerator<Token> tokenEnumerator, TokenType terminatingKeyword)
        {
            Expression.Builder expBuilder = Expression.CreateBuilder();
            expBuilder.ExtractTokenInfo(tokenEnumerator.Current);

            while(tokenEnumerator.Current.Type != terminatingKeyword)
            {
                expBuilder.Keyvalue = KeyValue.CreateBuilder();
                expBuilder.Keyvalue.ExtractTokenInfo(tokenEnumerator.Current);
                expBuilder.Keyvalue.Value = tokenEnumerator.Current.Text;
                if (!tokenEnumerator.MoveNext())
                {
                    //TODO: Error?
                }
            }
            expBuilder.Length = tokenEnumerator.Current.FullStart - expBuilder.FullStartPosition;
            return expBuilder;
        }

        private static ChunkNode ParseChunkNode(IEnumerator<Token> tokenEnumerator)
        {
            ChunkNode.Builder chunkNodeBuilder = ChunkNode.CreateBuilder();
            chunkNodeBuilder.ExtractTokenInfo(tokenEnumerator.Current);
            chunkNodeBuilder.ProgramBlock = ParseBlock(tokenEnumerator, TokenType.EndOfFile);
            chunkNodeBuilder.EndOfFile = ParseEndOfFile(tokenEnumerator);
            chunkNodeBuilder.Length = tokenEnumerator.Current.FullStart - chunkNodeBuilder.FullStartPosition;
            return chunkNodeBuilder.ToImmutable();
        }

        private static EndOfFileNode.Builder ParseEndOfFile(IEnumerator<Token> tokenEnumerator)
        {
            EndOfFileNode.Builder chunkNodeBuilder = EndOfFileNode.CreateBuilder();
            chunkNodeBuilder.ExtractTokenInfo(tokenEnumerator.Current);
            return chunkNodeBuilder;
        }

        private static Block.Builder ParseBlock(IEnumerator<Token> tokenEnumerator, TokenType terminatingType)
        {
            Block.Builder blockBuilder = Block.CreateBuilder();
            blockBuilder.ExtractTokenInfo(tokenEnumerator.Current);
            while(tokenEnumerator.Current.Type != terminatingType && tokenEnumerator.Current.Type != TokenType.EndOfFile)
            {
                blockBuilder.Children.Add(ParseStatement(tokenEnumerator));
                if (!tokenEnumerator.MoveNext())
                {
                    //TODO: Error?
                }
            }
            blockBuilder.Length = tokenEnumerator.Current.FullStart - blockBuilder.FullStartPosition;

            return blockBuilder;
        }

        private static Block.Builder ParseIfBlock(IEnumerator<Token> tokenEnumerator)
        {
            Block.Builder ifBlockBuilder = Block.CreateBuilder();
            ifBlockBuilder.ExtractTokenInfo(tokenEnumerator.Current);
            while ((tokenEnumerator.Current.Type != TokenType.ElseKeyword 
            || tokenEnumerator.Current.Type != TokenType.ElseIfKeyword
            || tokenEnumerator.Current.Type != TokenType.EndKeyword) 
            && tokenEnumerator.Current.Type != TokenType.EndOfFile) //TODO: Formating?
            {
                ifBlockBuilder.Children.Add(ParseStatement(tokenEnumerator));
                if (!tokenEnumerator.MoveNext())
                {
                    //TODO: Error?
                }
            }
            ifBlockBuilder.Length = tokenEnumerator.Current.FullStart - ifBlockBuilder.FullStartPosition;

            return ifBlockBuilder;
        }
    }
}