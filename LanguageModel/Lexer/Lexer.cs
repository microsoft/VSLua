// Lua Lexer
//TODO: peek 2 extension method
// "skipped" tokens add to trivia rather than having unknown token, 
// jump tables for is valid character of any kind, whitespace, digit, letter/underscore
// use stream not MoveableStreamReader

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LanguageModel
{
    internal class Lexer
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "and",
            "break",
            "do",
            "else",
            "elseif",
            "end",
            "false",
            "for",
            "function",
            "goto",
            "if",
            "in",
            "local",
            "nil",
            "not",
            "or",
            "repeat",
            "return",
            "then",
            "true",
            "until",
            "while"
        };

        private static readonly Dictionary<string, Token.TokenType> Symbols = new Dictionary<string, Token.TokenType>
        {
            { "-", Token.TokenType.Operator },
            { "~", Token.TokenType.Operator },
            { "#", Token.TokenType.Operator },
            {"~=", Token.TokenType.Operator },
            {"<=", Token.TokenType.Operator },
            {">=", Token.TokenType.Operator },
            {"==", Token.TokenType.Operator },
            {"+", Token.TokenType.Operator },
            {"*", Token.TokenType.Operator },
            {"/", Token.TokenType.Operator },
            {"//", Token.TokenType.Operator },
            {"^", Token.TokenType.Operator },
            {"%", Token.TokenType.Operator },
            {"&", Token.TokenType.Operator },
            {"|", Token.TokenType.Operator },
            {">>", Token.TokenType.Operator },
            {"<<", Token.TokenType.Operator },
            {"..", Token.TokenType.Operator },
            {">", Token.TokenType.Operator },
            {"<", Token.TokenType.Operator },
            {"=", Token.TokenType.Operator },

            {"{", Token.TokenType.OpenCurlyBrace },
            {"}", Token.TokenType.CloseCurlyBrace },
            {"(", Token.TokenType.OpenParen },
            {")", Token.TokenType.CloseParen },
            {"[", Token.TokenType.OpenBracket },
            {"]", Token.TokenType.CloseBracket },

            {".", Token.TokenType.Punctuation},
            {",", Token.TokenType.Punctuation},
            {";", Token.TokenType.Punctuation},
            {":", Token.TokenType.Punctuation},
            {"::", Token.TokenType.Punctuation}
        };

        private const char Eof = unchecked((char)-1);
        private readonly char[] longCommentID1 = { '-', '[','[' };
        private readonly char[] longCommentID2 = { '-', '[', '=' };

        public List<Token> Tokenize(Stream stream)
        {
            List<Token> tokens = new List<Token>();
            Token nextToken;
            List<Trivia> trivia;
            

            while (!stream.EndOfStream())
            {
				
				int fullStart = (int) stream.Position;
				trivia = this.ConsumeTrivia(stream);
                
                // TODO: return longest string of acceptable values (124fut return number 124)
                nextToken = this.ReadNextToken(stream, trivia, fullStart);

				tokens.Add(nextToken);
            }
			
			// TODO: IEnum yield return
			return tokens;
        } 

        private Token ReadNextToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            if (stream.EndOfStream()) //todo why hiting here?
            {
                return new Token(Token.TokenType.EndOfFile, "", trivia, fullStart, (int) stream.Position);
            }

            char nextChar = stream.Peek();

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

        private Token ReadAlphaToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            // Keyword or Identifier
            char nextChar;
            StringBuilder word = new StringBuilder();
            int tokenStartPosition = (int) stream.Position;
            do
            {
                word.Append(stream.ReadChar());
                nextChar = stream.Peek();
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
        
        private Token ReadNumberToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            StringBuilder number = new StringBuilder();
            int tokenStartPosition = (int) stream.Position;
            char next = stream.Peek();
            // TODO: verify only one decimal point

            while (this.IsValidNumber(next)) 
            {
                number.Append(stream.ReadChar());
                next = stream.Peek();
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

        private Token ReadStringToken(Stream stream, List<Trivia> leadingTrivia, int fullStart)
        {
            StringBuilder fullString = new StringBuilder();
            int tokenStartPosition = (int)stream.Position;
            char nextChar = stream.Peek(); 

            switch (nextChar)
            {
                case '"':
                    do
                    {
                        fullString.Append(stream.ReadChar());
                        nextChar = stream.Peek();

                    } while ((nextChar != '"') && !stream.EndOfStream());

                    if (nextChar == '"')
                    {
                        fullString.Append(stream.ReadChar());
                        return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream())
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
                        fullString.Append(stream.ReadChar());
                        nextChar = stream.Peek();

                    } while ((nextChar != '\'') && (!stream.EndOfStream()));

                    if (nextChar == '\'')
                    {
                        fullString.Append(stream.ReadChar());
                        return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream())
                        {
                            return new Token(Token.TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            return new Token(Token.TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                    }
                default:
                    fullString.Append(stream.ReadChar());
                    bool terminated = false;
                    switch (stream.Peek())
                    {
                        case '[':
                            fullString.Append(stream.ReadChar());

                            nextChar = stream.Peek();

                            while (!terminated && !stream.EndOfStream())
                            {
                                if(nextChar == ']')
                                {
                                    fullString.Append(stream.ReadChar());
                                    nextChar = stream.Peek();
                                    if (nextChar == ']')
                                    {
                                        fullString.Append(stream.ReadChar());
                                        terminated = true;
                                    }
                                    else
                                    {
                                        fullString.Append(stream.ReadChar());
                                        nextChar = stream.Peek();
                                    }

                                }
                                else
                                {
                                    fullString.Append(stream.ReadChar());
                                    nextChar = stream.Peek();
                                }
                            }
                            return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        case '=':
                            fullString.Append(stream.ReadChar());
                            int level = 1;
                            
                            nextChar = stream.Peek();

                            // Get levels (=) 
                            while (nextChar == '=')
                            {
                                fullString.Append(stream.ReadChar());
                                level++;
                                nextChar = stream.Peek();
                            }

                            if(nextChar == '[')
                            {
                                fullString.Append(stream.ReadChar());
                                nextChar = stream.Peek();

                                while (!terminated && !stream.EndOfStream())
                                {
                                    if(nextChar == ']')
                                    {
                                        fullString.Append(stream.ReadChar());
                                        nextChar = stream.Peek();
                                        int currentLevel = level;

                                        while(nextChar == '=')
                                        {
                                            fullString.Append(stream.ReadChar());
                                            level--;
                                            nextChar = stream.Peek();
                                        }

                                        if((nextChar == ']') && (level == 0) )
                                        {
                                            fullString.Append(stream.ReadChar());
                                            return new Token(Token.TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                                        }
                                    }
                                    else
                                    {
                                        fullString.Append(stream.ReadChar());
                                    }
                                    nextChar = stream.Peek();
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

        private Token ReadSymbolToken(Stream stream, List<Trivia> leadingTrivia, int fullStart)
        {
            int tokenStartPosition = (int) stream.Position;
            char nextChar = stream.ReadChar();

            switch (nextChar)
            {
                case ':':
                case '.':
                    // here use dictionary for minux, plus etc
                    if(nextChar != stream.Peek())
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
                    if ((nextChar != stream.Peek()) && (stream.Peek()!= '='))
                    {
                        return new Token(Token.TokenType.Operator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char secondOperatorChar = stream.ReadChar();
                        char[] symbol = { nextChar, secondOperatorChar };
                        return new Token(Token.TokenType.Operator, symbol.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '=':
                case '/':
                    if (nextChar != stream.Peek())
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
                    if (stream.Peek() != '=')
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

        private List<Trivia> ConsumeTrivia(Stream stream)
        {
            List<Trivia> triviaList = new List<Trivia>();
            bool isTrivia = false;

            char next;

            do
            {
                next = stream.Peek();
                
                switch (next)
                {
                    case ' ':
                    case '\t':
                        isTrivia = true;
                        triviaList.Add(CollectWhitespace(stream));
                        break;

					case '\r':
                    case '\n':
                        isTrivia = true;
                        Trivia newLineTrivia = new Trivia(Trivia.TriviaType.Newline, stream.ReadChar().ToString());
                        triviaList.Add(newLineTrivia);
                        break;

                    /*case '\r': This is just completely redundant IMO.
                        isTrivia = true;
                        stream.ReadChar();
                        next = stream.Peek();

                        if (next == '\n')
                        {
                            stream.ReadChar();
                        }

                        Trivia returnTrivia = new Trivia(Trivia.TriviaType.Newline, Environment.NewLine);
                        triviaList.Add(returnTrivia);
                        break;*/

                    case '-':
                                                
                        stream.ReadChar();

                        if(stream.Peek() == '-')
                        {
                            isTrivia = true;

							char[] currentCommentID = { stream.Peek(1), stream.Peek(2), stream.Peek(3) };

                            //int charsRead = stream.Read(currentCommentID, 0, longCommentID1.Length);

                            if (currentCommentID.SequenceEqual(longCommentID1) || (currentCommentID.SequenceEqual(longCommentID2)))
                            {
								stream.Read(currentCommentID, 0, longCommentID1.Length);
                                triviaList.Add(ReadLongComment(stream, currentCommentID));
                            }
                            else
                            {
								
                                triviaList.Add(ReadLineComment(stream, new char[]{ }));
                            }
                        }
                        else
                        {
                            isTrivia = false;
                            //stream.Position--;
                        }
                        break;

                    default:
                        isTrivia = false;
                        break;
                }

            } while (isTrivia);

            return triviaList;
        }

        private Trivia CollectWhitespace(Stream stream)
        {
            StringBuilder whitespace = new StringBuilder();
            whitespace.Append(stream.ReadChar());

            while (char.IsWhiteSpace(stream.Peek()))
            {
                whitespace.Append(stream.ReadChar());
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

        Trivia ReadLineComment(Stream stream, char[] commentRead)
        {
            string comment = "-" + new string(commentRead);

			while (stream.Peek() != '\n' && stream.Peek() != '\r' && stream.Peek() != Eof) // Todo: maybe not the safest way of checking for newline
			{
				 comment += stream.ReadChar();
			}
			
            return new Trivia(Trivia.TriviaType.Comment, comment);
        }

        Trivia ReadLongComment(Stream stream, char[] commentRead)
        {
            StringBuilder comment = new StringBuilder();
            comment.Append("-").Append(commentRead);

            int level = 0;
            char next;

            switch (commentRead[commentRead.Length - 1])
            {
                case '=':
                    level++;
                    next = stream.Peek();

                    // Get levels (=) 
                    while (next == '=')
                    {
                        comment.Append(stream.ReadChar());
                        level++;
                        next = stream.Peek();
                    }

                    if(next == '[')
                    {
                        comment.Append(stream.ReadChar());

                        while(level != 0)
                        {
                            next = stream.Peek();

                            if (next == ']')
                            {
                                comment.Append(stream.ReadChar());
                                int currentLevel = level;
                                next = stream.Peek();

                                while((next == '=') && (currentLevel > 0))
                                {
                                    comment.Append(stream.ReadChar());
                                    currentLevel--;
                                    next = stream.Peek();
                                }

                                if((next == ']') && (currentLevel == 0))
                                {
                                    comment.Append(stream.ReadChar());
                                    level = 0;
                                    return new Trivia(Trivia.TriviaType.Comment, comment.ToString());
                                }
                            }
                            else
                            {
                                comment.Append(stream.ReadChar());
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
                    comment.Append(stream.ReadChar());
                    char[] validEnd = { ']', ']' };
                    char[] currentEnd = new char[validEnd.Length];
                    stream.Read(currentEnd, 0, validEnd.Length);

                    while(currentEnd != validEnd)
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
			// TODO: rewrite to truly validate numbers - considering using regex
			// 1....2 <- not a number
			// 1e <- not a number
			// 1e-1 <- number
			// 1e+1 <- number
			// 1exexexexe4 <- not a number
        }
    }
}
