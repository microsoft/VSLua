using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LanguageService
{
    public static class Lexer
    {
        private static readonly Dictionary<string, TokenType> AlphaTokens = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AndBinop },
            { "break", TokenType.BreakKeyword },
            { "do", TokenType.DoKeyword },
            { "else", TokenType.ElseKeyword },
            { "elseif", TokenType.ElseIfKeyword },
            { "end", TokenType.EndKeyword },
            { "false", TokenType.FalseKeyValue },
            { "for", TokenType.ForKeyword },
            { "function", TokenType.FunctionKeyword },
            { "goto", TokenType.GotoKeyword },
            { "if", TokenType.IfKeyword },
            { "in", TokenType.InKeyword },
            { "local", TokenType.LocalKeyword },
            { "nil", TokenType.NilKeyValue },
            { "not",  TokenType.NotUnop },
            { "or",  TokenType.OrBinop },
            { "repeat", TokenType.RepeatKeyword },
            { "return", TokenType.ReturnKeyword },
            { "then", TokenType.ThenKeyword },
            { "true", TokenType.TrueKeyValue },
            { "until", TokenType.UntilKeyword },
            { "while", TokenType.WhileKeyword }
        };

        private static readonly Dictionary<string, TokenType> Symbols = new Dictionary<string, TokenType>
        {
            //TODO: include vararg operators
            { "-", TokenType.MinusOperator }, //TODO: deal with ambiguity
            { "~", TokenType.TildeUnOp },
            { "#", TokenType.LengthUnop },
            {"~=", TokenType.NotEqualsOperator },
            {"<=", TokenType.LessOrEqualOperator },
            {">=", TokenType.GreaterOrEqualOperator },
            {"==", TokenType.EqualityOperator },
            {"+", TokenType.PlusOperator },
            {"*", TokenType.MultiplyOperator },
            {"/", TokenType.DivideOperator },
            {"//", TokenType.FloorDivideOperator },
            {"^", TokenType.ExponentOperator },
            {"%", TokenType.ModulusOperator },
            {"&", TokenType.BitwiseAndOperator },
            {"|", TokenType.BitwiseOrOperator },
            {">>", TokenType.BitwiseRightOperator },
            {"<<", TokenType.BitwiseLeftOperator },
            {"..", TokenType.StringConcatOperator },
            {">", TokenType.GreaterThanOperator },
            {"<", TokenType.LessThanOperator },
            {"=", TokenType.AssignmentOperator },

            {"{", TokenType.OpenCurlyBrace },
            {"}", TokenType.CloseCurlyBrace },
            {"(", TokenType.OpenParen },
            {")", TokenType.CloseParen },
            {"[", TokenType.OpenBracket },
            {"]", TokenType.CloseBracket },

            {".", TokenType.Dot},
            {",", TokenType.Comma},
            {";", TokenType.SemiColon},
            {":", TokenType.Colon},
            {"::", TokenType.DoubleColon}
        };

        private const char Eof = unchecked((char)-1);
        private static readonly char[] longCommentID1 = { '-', '[','[' };
        private static readonly char[] longCommentID2 = { '-', '[', '=' }; //TODO: flawed approach? what if --[=asdfadf]?

        public static Dictionary<string, TokenType> Symbols1
        {
            get
            {
                return Symbols;
            }
        }

        public static List<Token> Tokenize(Stream stream) //TODO: Return a bool based on if this is a new copy of the lexer or not
        {
            Token nextToken;
            List<Trivia> trivia;

            List<Token> tokenList = new List<Token>();

            while (!stream.EndOfStream())
            {
                int fullStart = (int)stream.Position;
                trivia = ConsumeTrivia(stream);
                nextToken = ReadNextToken(stream, trivia, fullStart);
                tokenList.Add(nextToken);

                if (stream.EndOfStream() && nextToken.Type != TokenType.EndOfFile)
                {
                    nextToken = new Token(TokenType.EndOfFile, "", new List<Trivia>(), fullStart, (int)stream.Position);
                    tokenList.Add(nextToken);
                }
            }
            return tokenList;
        }
        private static List<Trivia> ConsumeTrivia(Stream stream)
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
                    case '\n':
                        isTrivia = true;
                        Trivia newLineTrivia = new Trivia(Trivia.TriviaType.Newline, stream.ReadChar().ToString());
                        triviaList.Add(newLineTrivia);
                        break;

                   case '\r': //TODO: Is this is just completely redundant IMO.
                        isTrivia = true;
                        stream.ReadChar();
                        next = stream.Peek();

                        Trivia returnTrivia;

                        if (next == '\n')
                        {
                            stream.ReadChar();
                            returnTrivia = new Trivia(Trivia.TriviaType.Newline, "\r\n");
                        } else
                        {
                            returnTrivia = new Trivia(Trivia.TriviaType.Newline, "\r");
                        }

                        triviaList.Add(returnTrivia);
                        break;

                    case '-':
                                                
                        stream.ReadChar();

                        if(stream.Peek() == '-')
                        {
                            isTrivia = true;

							char[] currentCommentID = { stream.Peek(1), stream.Peek(2), stream.Peek(3) };

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
                            stream.Position--;
                        }
                        break;

                    default:
                        isTrivia = false;
                        break;
                }

            } while (isTrivia);

            return triviaList;
        }

        private static Token ReadNextToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            char nextChar;

            if (stream.EndOfStream())
            {
                return new Token(TokenType.EndOfFile, "", trivia, fullStart, (int)stream.Position);
            }

            nextChar = stream.Peek();

            // Keyword or Identifier
            if (char.IsLetter(nextChar) || (nextChar == '_'))
            {
                return ReadAlphaToken(stream, trivia, fullStart);
            }
            // Number
            else if (char.IsDigit(nextChar))
            {
               return ReadNumberToken(stream, trivia, fullStart);
            }
            // String
            else if (IsQuote(nextChar))
            {
                return ReadStringToken(stream, trivia, fullStart);
            }
            // Punctuation Bracket Operator
            else
            {
                return ReadSymbolToken(stream, trivia, fullStart);
            } 
        }
        private static Token ReadAlphaToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            // Keyword or Identifier
            char nextChar;
            StringBuilder word = new StringBuilder();
            int tokenStartPosition = (int) stream.Position;
            do
            {
                word.Append(stream.ReadChar());
                nextChar = stream.Peek();
            } while (IsAlphaCharacter(nextChar));

            string value = word.ToString();
            
            if (AlphaTokens.ContainsKey(value))
            {
                return new Token(AlphaTokens[value], value, trivia, fullStart, tokenStartPosition);
            }
            else
            {
                return new Token(TokenType.Identifier, value, trivia, fullStart, tokenStartPosition);
            }
        }

        private static Token ReadNumberToken(Stream stream, List<Trivia> trivia, int fullStart)
        {
            StringBuilder number = new StringBuilder();
            int tokenStartPosition = (int) stream.Position;
            char next = stream.Peek();
            // TODO: verify only one decimal point

            while (IsValidNumber(next)) 
            {
                number.Append(stream.ReadChar());
                next = stream.Peek();
            }

            if (IsValidTerminator(next) || stream.EndOfStream())
            {
                return new Token(TokenType.Number, number.ToString(), trivia, fullStart, tokenStartPosition);
            }
            else
            {
                return new Token(TokenType.Unknown, number.ToString(), trivia, fullStart, tokenStartPosition); //TODO: Deal with invalid number/identifier: "234kjs"
            }
        }

        private static Token ReadStringToken(Stream stream, List<Trivia> leadingTrivia, int fullStart)
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
                        return new Token(TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream())
                        {
                            return new Token(TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            return new Token(TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
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
                        return new Token(TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (stream.EndOfStream())
                        {
                            return new Token(TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            return new Token(TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
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
                            return new Token(TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
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
                                            return new Token(TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                                        }
                                    }
                                    else
                                    {
                                        fullString.Append(stream.ReadChar());
                                    }
                                    nextChar = stream.Peek();
                                }

                                return new Token(TokenType.String, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);

                            }
                            else
                            {
                                // Error, not valid syntax
                                return new Token(TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                            }
                        default:
                            return new Token(TokenType.OpenBracket, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
            }
        }

        private static Token ReadSymbolToken(Stream stream, List<Trivia> leadingTrivia, int fullStart)
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
                        return new Token(Symbols[nextChar.ToString()], nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char [] symbol = { nextChar, nextChar };
                        string symbolKey = new string(symbol);
                        return new Token(Symbols[symbolKey], symbolKey, leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '<':
                case '>':
                    // could be doubles or eq sign
                    if ((nextChar != stream.Peek()) && (stream.Peek()!= '='))
                    {
                        return new Token(Symbols[nextChar.ToString()], nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char secondOperatorChar = stream.ReadChar();
                        char[] symbol = { nextChar, secondOperatorChar };
                        string symbolKey = new string(symbol);
                        return new Token(Symbols[symbolKey], symbolKey, leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '=':
                case '/':
                    if (nextChar != stream.Peek())
                    {
                        return new Token(Symbols[nextChar.ToString()], nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        stream.ReadChar();
                        char[] symbol = { nextChar, nextChar };
                        string symbolKey = new string(symbol);
                        return new Token(Symbols[symbolKey], symbolKey, leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '~':
                    if (stream.Peek() != '=')
                    {
                        return new Token(Symbols[nextChar.ToString()], nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        char[] symbol = { nextChar, '=' };
                        string symbolKey = new string(symbol);
                        return new Token(Symbols[symbolKey], symbolKey, leadingTrivia, fullStart, tokenStartPosition);
                    }
                default:
                    // non repeating symbol
                    string fullSymbol = nextChar.ToString();
                    if (Symbols1.ContainsKey(fullSymbol))
                    {
                        return new Token(Symbols1[fullSymbol], fullSymbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.Unknown, fullSymbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
            }
        }

        private static bool IsAlphaCharacter(char a)
        {
            return (char.IsLetter(a) || char.IsNumber(a) || (a == '_')); //TODO? Unicode?
        }


        private static Trivia CollectWhitespace(Stream stream)
        {
            StringBuilder whitespace = new StringBuilder();
            whitespace.Append(stream.ReadChar());

            while (char.IsWhiteSpace(stream.Peek()))
            {
                whitespace.Append(stream.ReadChar());
            }

            return new Trivia(Trivia.TriviaType.Whitespace, whitespace.ToString());
        }

        private static bool IsValidTerminator(char next)
        {
            return !char.IsLetter(next); //TODO: check if this is alright problems with isletter?
        }

        private static bool IsQuote(char nextChar)
        {
            return ((nextChar == '"') || (nextChar == '\'') || (nextChar == '['));
        }

        private static Trivia ReadLineComment(Stream stream, char[] commentRead)
        {
            string comment = "-" + new string(commentRead);

			while (stream.Peek() != '\n' && stream.Peek() != '\r' && stream.Peek() != Eof) // Todo: maybe not the safest way of checking for newline
			{
				 comment += stream.ReadChar();
			}
			
            return new Trivia(Trivia.TriviaType.Comment, comment);
        }

        private static Trivia ReadLongComment(Stream stream, char[] commentRead)
        {
            StringBuilder comment = new StringBuilder();
            comment.Append("-").Append(new string(commentRead));

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

        private static bool IsValidNumber(char character)
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

        public static void PrintTokens(Stream stream)
        {
            IEnumerable<Token> tokenEnumerable = Lexer.Tokenize(stream);
            foreach (Token t in tokenEnumerable)
            {
                Console.WriteLine(t);
            }
        }

    }
}
