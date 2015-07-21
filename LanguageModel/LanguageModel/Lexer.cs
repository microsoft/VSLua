using Microsoft.Internal.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageService
{
    public static class Lexer
    {
        private static readonly IReadOnlyDictionary<string, TokenType> AlphaTokens = new Dictionary<string, TokenType>
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

        private static readonly IReadOnlyDictionary<string, TokenType> Symbols = new Dictionary<string, TokenType>
        {
            //TODO: include vararg operators
            { "-", TokenType.MinusOperator }, //TODO: deal with ambiguity
            { "~", TokenType.TildeUnOp }, //TODO: deal with ambiguity
            { "#", TokenType.LengthUnop },
            { "~=", TokenType.NotEqualsOperator },
            { "<=", TokenType.LessOrEqualOperator },
            { ">=", TokenType.GreaterOrEqualOperator },
            { "==", TokenType.EqualityOperator },
            { "+", TokenType.PlusOperator },
            { "*", TokenType.MultiplyOperator },
            { "/", TokenType.DivideOperator },
            { "//", TokenType.FloorDivideOperator },
            { "^", TokenType.ExponentOperator },
            { "%", TokenType.ModulusOperator },
            { "&", TokenType.BitwiseAndOperator },
            { "|", TokenType.BitwiseOrOperator },
            { ">>", TokenType.BitwiseRightOperator },
            { "<<", TokenType.BitwiseLeftOperator },
            { "..", TokenType.StringConcatOperator },
            { ">", TokenType.GreaterThanOperator },
            { "<", TokenType.LessThanOperator },
            { "=", TokenType.AssignmentOperator },

            { "{", TokenType.OpenCurlyBrace },
            { "}", TokenType.CloseCurlyBrace },
            { "(", TokenType.OpenParen },
            { ")", TokenType.CloseParen },
            { "[", TokenType.OpenBracket },
            { "]", TokenType.CloseBracket },

            { ".", TokenType.Dot},
            { ",", TokenType.Comma},
            { ";", TokenType.Semicolon},
            { ":", TokenType.Colon},
            { "::", TokenType.DoubleColon}
        };

        private const char Eof = unchecked((char)-1);
        private static readonly char[] longCommentID1 = { '-', '[', '[' };
        private static readonly char[] longCommentID2 = { '-', '[', '=' }; //TODO: flawed approach? what if --[=asdfadf]?

        public static List<Token> Tokenize(Stream stream) //TODO: Return a bool based on if this is a new copy of the lexer or not
        {
            Validate.IsNotNull(stream, nameof(stream));

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
                        }
                        else
                        {
                            returnTrivia = new Trivia(Trivia.TriviaType.Newline, "\r");
                        }

                        triviaList.Add(returnTrivia);
                        break;

                    case '-':

                        stream.ReadChar();

                        if (stream.Peek() == '-')
                        {
                            isTrivia = true;
                            stream.ReadChar();
                            string commentSoFar = "";

                            int? level = Lexer.GetLongCommentOpenBracket(stream, ref commentSoFar);

                            if (level != null)
                            {
                                triviaList.Add(ReadLongComment(stream, "--" + commentSoFar, level));
                            }
                            else
                            {
                                triviaList.Add(ReadLineComment(stream, new char[] { '-', '-' }));
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

        private static Trivia ReadLongComment(Stream stream, string commentSoFar, int? level)
        {
            Validate.IsNotNull(level, nameof(level));

            //TODO: re-write without regex
            Regex closeBracketPattern = new Regex(@"\]={" + level.ToString() + @"}\]");

            while (!closeBracketPattern.IsMatch(commentSoFar))
            {
                commentSoFar += stream.ReadChar();
            }

            return new Trivia(Trivia.TriviaType.Comment, commentSoFar);
        }

        private static int? GetLongCommentOpenBracket(Stream stream, ref string commentSoFar)
        {
            if (stream.Peek() != '[')
            {
                return null;
            }
            Regex openBracketPattern = new Regex(@"=*\[");
            int previousMatchLength = 0;
            commentSoFar += stream.ReadChar();
            while (true)
            {
                char c = stream.Peek();
                // I just need something to work right now... (don't really care for efficiency)
                Match match = openBracketPattern.Match(commentSoFar);
                if (match.Length > previousMatchLength)
                {
                    commentSoFar += c;
                    previousMatchLength = match.Length;
                    stream.ReadChar();
                    if (c == '[')
                    {
                        return commentSoFar.Length - 2;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
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
                return ReadStringToken(nextChar, stream, trivia, fullStart);
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
            int tokenStartPosition = (int)stream.Position;
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
            int tokenStartPosition = (int)stream.Position;
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

        private static Token ReadStringToken(char stringDelimiter, Stream stream, List<Trivia> leadingTrivia, int fullStart)
        {
            StringBuilder fullString = new StringBuilder();
            int tokenStartPosition = (int)stream.Position;
            TokenType type = TokenType.String;
            char nextChar;

            switch (stringDelimiter)
            {
                case '"':
                case '\'':
                    fullString.Append(stream.ReadChar());
                    nextChar = stream.Peek();
                    bool terminateString = false;
                    while ((nextChar != stringDelimiter) && !stream.EndOfStream() && !terminateString)
                    {
                        fullString.Append(stream.ReadChar());
                        nextChar = stream.Peek();

                        if (nextChar == '\r' || nextChar == '\n')
                        {
                            type = TokenType.UnterminatedString;
                            terminateString = true;
                        }
                    }

                    if (nextChar == stringDelimiter || terminateString)
                    {
                        fullString.Append(stream.ReadChar());
                        return new Token(type, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition); //TODO bug... should return a string then a EOF token right?
                    }

                case '[':
                    fullString.Append(stream.ReadChar());
                    int bracketLevel = 0;
                    nextChar = stream.Peek();

                    bracketLevel = CountLevels(false, nextChar, 0, stream, fullString);
                    nextChar = stream.Peek();

                    if (nextChar == '[')
                    {
                        fullString.Append(stream.ReadChar());
                        nextChar = stream.Peek();

                        //Lua ignores a new line directly after the opening delimiter of a string.
                        if (nextChar == '\r' || nextChar == '\n')
                        {
                            if (nextChar == '\r')
                                stream.ReadChar();
                            if (stream.Peek() == '\n')
                                stream.ReadChar();
                            type = TokenType.IgnoreNewLineString;
                        }

                        while (!stream.EndOfStream())
                        {
                            if (nextChar == ']')
                            {
                                fullString.Append(stream.ReadChar());
                                nextChar = stream.Peek();
                                int currentLevel = bracketLevel;

                                currentLevel = CountLevels(true, nextChar, currentLevel, stream, fullString);
                                nextChar = stream.Peek();

                                if ((nextChar == ']') && (currentLevel == 0))
                                {
                                    fullString.Append(stream.ReadChar());
                                    return new Token(type, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                                }
                            }
                            else
                            {
                                fullString.Append(stream.ReadChar());
                            }
                            nextChar = stream.Peek();
                        }

                        return new Token(TokenType.UnterminatedString, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        if (bracketLevel == 0)
                        {
                            return new Token(TokenType.OpenBracket, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                            // Error, not valid syntax
                            return new Token(TokenType.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(stringDelimiter), "Unrecognized String delimiter");
            }
        }

        private static Token ReadSymbolToken(Stream stream, List<Trivia> leadingTrivia, int fullStart)
        {
            int tokenStartPosition = (int)stream.Position;
            char nextChar = stream.ReadChar();

            switch (nextChar)
            {
                case ':':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(TokenType.Colon, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.DoubleColon, "::", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '.':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(TokenType.Dot, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.StringConcatOperator, "..", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '<':
                    if ((nextChar != stream.Peek()) && (stream.Peek() != '='))
                    {
                        return new Token(TokenType.LessThanOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        string symbol = nextChar.ToString() + stream.ReadChar();
                        return new Token(Symbols[symbol], symbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '>':
                    if ((nextChar != stream.Peek()) && (stream.Peek() != '='))
                    {
                        return new Token(Symbols[nextChar.ToString()], nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        string symbol = nextChar.ToString() + stream.ReadChar();
                        return new Token(Symbols[symbol], symbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '=':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(TokenType.AssignmentOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.EqualityOperator, "==", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '/':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(TokenType.DivideOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.FloorDivideOperator, "//", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '~':
                    if (CheckAndConsumeNextToken('=', stream))
                    {
                        return new Token(TokenType.TildeUnOp, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(TokenType.NotEqualsOperator, "~=", leadingTrivia, fullStart, tokenStartPosition);
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

            while (stream.Peek() == ' ' || stream.Peek() == '\t') // Question: are there any other types of whitespace?
            {
                whitespace.Append(stream.ReadChar());
            }

            return new Trivia(Trivia.TriviaType.Whitespace, whitespace.ToString());
        }

        private static bool IsValidTerminator(char next)
        {
            return !char.IsLetter(next); //Question are there any problems with isletter?
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

        public static bool CheckAndConsumeNextToken(char character, Stream stream)
        {
            if (character != stream.Peek())
            {
                return true;
            }
            else
            {
                stream.ReadChar();
                return false;
            }
        }

        public static int CountLevels(bool validateCount, char character, int counter, Stream stream, StringBuilder builder)
        {
            int levelCount = counter;
            while (character == '=' && !stream.EndOfStream())
            {
                builder.Append(stream.ReadChar());
                if (validateCount)
                {
                    levelCount--;
                }
                else
                {
                    levelCount++;
                }

                character = stream.Peek();
            }
            return levelCount;
        }
    }
}
