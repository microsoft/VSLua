// Lua Lexer
//TODO: peek 2 extension method
// "skipped" tokens add to trivia rather than having unknown token, 
// jump tables for is valid character of any kind, whitespace, digit, letter/underscore
// use stream not MoveableStreamReader

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageModel
{
    internal class Lexer
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "and", //TODO: Return binop
            "break",
            "do",
            "else",
            "elseif",
            "end",
            "false", //TODO: return exp
            "for",
            "function",
            "goto",
            "if",
            "in",
            "local",
            "nil", //TODO: return exp
            "not", //TODO: return unop
            "or", //TODO: Return binop
            "repeat",
            "return",
            "then",
            "true", //TODO: return exp
            "until",
            "while"
        };

        private static readonly Dictionary<string, Token.TokenType> Symbols = new Dictionary<string, Token.TokenType>
        {
            {"-", Token.TokenType.Operator},
            {"~", Token.TokenType.Operator},
            {"#", Token.TokenType.Operator},
            {"~=", Token.TokenType.Operator},
            {"<=", Token.TokenType.Operator},
            {">=", Token.TokenType.Operator},
            {"==", Token.TokenType.Operator},
            {"+", Token.TokenType.Operator},
            {"*", Token.TokenType.Operator},
            {"/", Token.TokenType.Operator},
            {"//", Token.TokenType.Operator},
            {"^", Token.TokenType.Operator},
            {"%", Token.TokenType.Operator},
            {"&", Token.TokenType.Operator},
            {"|", Token.TokenType.Operator},
            {">>", Token.TokenType.Operator},
            {"<<", Token.TokenType.Operator},
            {"..", Token.TokenType.Operator},
            {">", Token.TokenType.Operator},
            {"<", Token.TokenType.Operator},
            {"=", Token.TokenType.Operator},

            {"{", Token.TokenType.OpenCurlyBrace},
            {"}", Token.TokenType.CloseCurlyBrace},
            {"(", Token.TokenType.OpenParen},
            {")", Token.TokenType.CloseParen},
            {"[", Token.TokenType.OpenBracket},
            {"]", Token.TokenType.CloseBracket},

            {".", Token.TokenType.Punctuation},
            {",", Token.TokenType.Punctuation},
            {";", Token.TokenType.Punctuation},
            {":", Token.TokenType.Punctuation},
            {"::", Token.TokenType.Punctuation}
        };

        private const char EOF = unchecked((char)-1);
        private readonly char[] longCommentID1 = { '-', '[', '[' };
        private readonly char[] longCommentID2 = { '-', '[', '=' };

        public List<Token> Tokenize(MoveableStreamReader stream)
        {
            List<Token> tokens = new List<Token>();
            Token nextToken;
            List<Trivia> trivia;
            int fullStart = stream.currentPositionInStream;

            while (!stream.EndOfStream)
            {
                trivia = this.ConsumeTrivia(stream);

                // TODO: return longest string of acceptable values (124fut return number 124)
                nextToken = this.ReadNextToken(stream, trivia, fullStart);

                tokens.Add(nextToken);
                Console.WriteLine(nextToken.ToString());
            }
            return tokens;
        }

        private Token ReadNextToken(MoveableStreamReader stream, List<Trivia> trivia, int fullStart)
        {
            if (stream.EndOfStream)
            {
                return new Token(Token.TokenType.EndOfFile, "", trivia, fullStart, stream.currentPositionInStream);
            }

            char nextChar = (char)stream.Peek();

            // Keyword or Identifier
            if (char.IsLetter(nextChar) || (nextChar == '_'))
            {
                return this.ReadAlphaToken(stream, trivia, fullStart);
            }
            // Number
            else if (char.IsDigit(nextChar))
            {
                return this.ReadNumberToken(stream, trivia, fullStart);
            }
            // String
            else if (IsQuote(nextChar))
            {
                return this.ReadStringToken(stream, trivia, fullStart);
            }
            // Punctuation Bracket Operator
            else
            {
                return this.ReadSymbolToken(stream, trivia, fullStart);
            }
        }

        private Token ReadAlphaToken(MoveableStreamReader stream, List<Trivia> trivia, int fullStart)
        {
            // Keyword or Identifier
            char nextChar;
            StringBuilder word = new StringBuilder();
            int tokenStartPosition = stream.currentPositionInStream;
            do
            {
                word.Append((char)stream.Read());
                nextChar = (char)stream.Peek();
            } while (this.IsAlphaCharacter(nextChar));

            string value = word.ToString();

            if (Keywords.Contains(value))
            {
                return new Token(Token.TokenType.Keyword, value, trivia, fullStart, tokenStartPosition);
            }
            else
            {
                return new Token(Token.TokenType.Identifier, value, trivia, fullStart, tokenStartPosition);
            }

        }

        private Token ReadNumberToken(MoveableStreamReader stream, List<Trivia> trivia, int fullStart)
        {
            StringBuilder number = new StringBuilder();
            int tokenStartPosition = stream.currentPositionInStream;
            char next = (char)stream.Peek();
            // TODO: verify only one decimal point

            while (this.IsValidNumber(next))
            {
                number.Append((char)stream.Read());
                next = (char)stream.Peek();
            }

            if (this.IsValidTerminator(next))
            {
                return new Token(Token.TokenType.Number, number.ToString(), trivia, fullStart, tokenStartPosition);
            }
            else
            {
                return new Token(Token.TokenType.Unknown, number.ToString(), trivia, fullStart, tokenStartPosition);
            }
        }

        private Token ReadStringToken(MoveableStreamReader stream, List<Trivia> leadingTrivia, int fullStart)
        {
            StringBuilder fullString = new StringBuilder();
            int tokenStartPosition = stream.currentPositionInStream;
            char nextChar = (char)stream.Peek();

            switch (nextChar)
            {
                case '"':
                    do
                    {
                        fullString.Append((char)stream.Read());
                        nextChar = (char)stream.Peek();

                    } while ((nextChar != '"') && !stream.EndOfStream);

                    if (nextChar == '"')
                    {
                        fullString.Append((char)stream.Read());
                        return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream)
                        {
                            return new Token(Token.TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            return new Token(Token.TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }

                    }
                case '\'':
                    do
                    {
                        fullString.Append((char)stream.Read());
                        nextChar = (char)stream.Peek();

                    } while ((nextChar != '\'') && (!stream.EndOfStream));

                    if (nextChar == '\'')
                    {
                        fullString.Append((char)stream.Read());
                        return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream)
                        {
                            return new Token(Token.TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            return new Token(Token.TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                    }
                default:
                    fullString.Append((char)stream.Read());
                    bool terminated = false;
                    switch ((char)stream.Peek())
                    {
                        case '[':
                            fullString.Append((char)stream.Read());

                            nextChar = (char)stream.Peek();

                            while (!terminated && !stream.EndOfStream)
                            {
                                if (nextChar == ']')
                                {
                                    fullString.Append((char)stream.Read());
                                    nextChar = (char)stream.Peek();
                                    if (nextChar == ']')
                                    {
                                        fullString.Append((char)stream.Read());
                                        terminated = true;
                                    }
                                    else
                                    {
                                        fullString.Append((char)stream.Read());
                                        nextChar = (char)stream.Peek();
                                    }

                                }
                                else
                                {
                                    fullString.Append((char)stream.Read());
                                    nextChar = (char)stream.Peek();
                                }
                            }
                            return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        case '=':
                            fullString.Append((char)stream.Read());
                            int level = 1;

                            nextChar = (char)stream.Peek();

                            // Get levels (=) 
                            while (nextChar == '=')
                            {
                                fullString.Append((char)stream.Read());
                                level++;
                                nextChar = (char)stream.Peek();
                            }

                            if (nextChar == '[')
                            {
                                fullString.Append((char)stream.Read());
                                nextChar = (char)stream.Peek();

                                while (!terminated && !stream.EndOfStream)
                                {
                                    if (nextChar == ']')
                                    {
                                        fullString.Append((char)stream.Read());
                                        nextChar = (char)stream.Peek();
                                        int currentLevel = level;

                                        while (nextChar == '=')
                                        {
                                            fullString.Append((char)stream.Read());
                                            level--;
                                            nextChar = (char)stream.Peek();
                                        }

                                        if ((nextChar == ']') && (level == 0))
                                        {
                                            fullString.Append((char)stream.Read());
                                            return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                                        }
                                    }
                                    else
                                    {
                                        fullString.Append((char)stream.Read());
                                    }
                                    nextChar = (char)stream.Peek();
                                }

                                return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);

                            }
                            else
                            {
                                // Error, not valid syntax
                                return new Token(Token.TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                            }
                        default:
                            return new Token(Token.TokenType.OpenBracket, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
            }
        }

        private Token ReadSymbolToken(MoveableStreamReader stream, List<Trivia> leadingTrivia, int fullStart)
        {
            int tokenStartPosition = stream.currentPositionInStream;
            char nextChar = (char)stream.Read();

            switch (nextChar)
            {
                case ':':
                case '.':
                    // here use dictionary for minux, plus etc
                    if (nextChar != (char)stream.Peek())
                    {
                        return new Token(Token.TokenType.Punctuation, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char[] symbol = { nextChar, nextChar };
                        return new Token(Token.TokenType.Punctuation, symbol.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '<':
                case '>':
                    // could be doubles or eq sign
                    if ((nextChar != (char)stream.Peek()) && ((char)stream.Peek() != '='))
                    {
                        return new Token(Token.TokenType.Operator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char secondOperatorChar = (char)stream.Read();
                        char[] symbol = { nextChar, secondOperatorChar };
                        return new Token(Token.TokenType.Operator, symbol.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '=':
                case '/':
                    if (nextChar != (char)stream.Peek())
                    {
                        return new Token(Token.TokenType.Operator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char[] symbol = { nextChar, nextChar };
                        //string symbol = char.ToString(nextChar) + char.ToString(nextChar);
                        return new Token(Token.TokenType.Operator, new string(symbol), leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '~':
                    if ((char)stream.Peek() != '=')
                    {
                        return new Token(Token.TokenType.Operator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char[] symbol = { nextChar, '=' };
                        return new Token(Token.TokenType.Operator, new string(symbol), leadingTrivia, fullStart, tokenStartPosition);
                    }
                default:
                    // non repeating symbol
                    string fullSymbol = nextChar.ToString();
                    if (Symbols.ContainsKey(fullSymbol))
                    {
                        return new Token(Symbols[fullSymbol], fullSymbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(Token.TokenType.Unknown, fullSymbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
            }
        }

        private bool IsAlphaCharacter(char a)
        {
            return (char.IsLetter(a) || char.IsNumber(a) || (a == '_'));
        }

        private List<Trivia> ConsumeTrivia(MoveableStreamReader stream)
        {
            List<Trivia> triviaList = new List<Trivia>();
            bool isTrivia = false;

            char next;

            do
            {
                next = (char)stream.Peek();

                switch (next)
                {
                    case ' ':
                    case '\t':
                        isTrivia = true;
                        triviaList.Add(CollectWhitespace(stream));
                        break;

                    case '\n':
                        isTrivia = true;
                        Trivia newLineTrivia = new Trivia(Trivia.TriviaType.Newline, next.ToString());
                        triviaList.Add(newLineTrivia);
                        break;

                    case '\r':
                        isTrivia = true;
                        stream.Read();
                        next = (char)stream.Peek();

                        if (next == '\n')
                        {
                            stream.Read();
                        }

                        Trivia returnTrivia = new Trivia(Trivia.TriviaType.Newline, Environment.NewLine);
                        triviaList.Add(returnTrivia);
                        break;

                    case '-':

                        stream.Read();

                        if ((char)stream.Peek() == '-')
                        {
                            isTrivia = true;

                            char[] currentCommentID = new char[longCommentID1.Length];

                            int charsRead = stream.Read(currentCommentID, 0, longCommentID1.Length);

                            if (currentCommentID.SequenceEqual(longCommentID1) || (currentCommentID.SequenceEqual(longCommentID2)))
                            {
                                triviaList.Add(ReadLongComment(stream, currentCommentID));
                            }
                            else
                            {
                                triviaList.Add(ReadLineComment(stream, currentCommentID));
                            }
                        }
                        else
                        {
                            isTrivia = false;
                            stream.PushBack('-');
                        }
                        break;

                    default:
                        isTrivia = false;
                        break;
                }

            } while (isTrivia);

            return triviaList;
        }

        private Trivia CollectWhitespace(MoveableStreamReader stream)
        {
            StringBuilder whitespace = new StringBuilder();
            whitespace.Append((char)stream.Read());

            while (char.IsWhiteSpace((char)stream.Peek()))
            {
                whitespace.Append((char)stream.Read());
            }

            return new Trivia(Trivia.TriviaType.Whitespace, whitespace.ToString());
        }

        bool IsValidTerminator(char next)
        {
            //switch (next)
            //{
            //    case ';':
            //    case ' ':
            //    case '\n':
            //    case '\t':
            //    case ',':
            //    case Lexer.EOF:
            //        return true;
            //    default:
            //        return false;
            //}

            return !char.IsLetter(next); //TODO: check if this is alright problems with isletter?
        }

        bool IsQuote(char nextChar)
        {
            return ((nextChar == '"') || (nextChar == '\'') || (nextChar == '['));
        }

        Trivia ReadLineComment(MoveableStreamReader stream, char[] commentRead)
        {
            string comment = "-" + new string(commentRead);
            comment += stream.ReadLine();
            return new Trivia(Trivia.TriviaType.Comment, comment);
        }

        Trivia ReadLongComment(MoveableStreamReader stream, char[] commentRead)
        {
            StringBuilder comment = new StringBuilder();
            comment.Append("-").Append(new string(commentRead));

            int level = 0;
            char next;

            switch (commentRead[commentRead.Length - 1])
            {
                case '=':
                    level++;
                    next = (char)stream.Peek();

                    // Get levels (=) 
                    while (next == '=')
                    {
                        comment.Append((char)stream.Read());
                        level++;
                        next = (char)stream.Peek();
                    }

                    if (next == '[')
                    {
                        comment.Append((char)stream.Read());

                        while (level != 0)
                        {
                            next = (char)stream.Peek();

                            if (next == ']')
                            {
                                comment.Append((char)stream.Read());
                                int currentLevel = level;
                                next = (char)stream.Peek();

                                while ((next == '=') && (currentLevel > 0))
                                {
                                    comment.Append((char)stream.Read());
                                    currentLevel--;
                                    next = (char)stream.Peek();
                                }

                                if ((next == ']') && (currentLevel == 0))
                                {
                                    comment.Append((char)stream.Read());
                                    level = 0;
                                    return new Trivia(Trivia.TriviaType.Comment, comment.ToString());
                                }
                            }
                            else
                            {
                                comment.Append((char)stream.Read());
                            }
                        }
                    }
                    else
                    {
                        // TODO: fix that double type cast
                        char[] alreadyConsumed = comment.ToString().ToCharArray();

                        return ReadLineComment(stream, alreadyConsumed);
                    }

                    break;
                case '[':
                    comment.Append((char)stream.Read());
                    char[] validEnd = { ']', ']' };
                    char[] currentEnd = new char[validEnd.Length];
                    stream.Read(currentEnd, 0, validEnd.Length);

                    while (currentEnd != validEnd)
                    {
                        comment.Append(currentEnd);
                        stream.Read(currentEnd, 0, validEnd.Length);
                    }

                    comment.Append(currentEnd);
                    return new Trivia(Trivia.TriviaType.Comment, comment.ToString());
            }

            return new Trivia(Trivia.TriviaType.Comment, comment.ToString());
        }

        bool IsValidNumber(char character)
        {
            // switch 1-9, . , e, x
            return (char.IsDigit(character) || (character == '.') || (character == 'e') || (character == 'x'));
        }
    }
}
