using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Validation;

namespace LanguageService
{
    public static class Lexer
    {
        private static readonly IReadOnlyDictionary<string, SyntaxKind> AlphaTokens = new Dictionary<string, SyntaxKind>
        {
            { "and", SyntaxKind.AndBinop },
            { "break", SyntaxKind.BreakKeyword },
            { "do", SyntaxKind.DoKeyword },
            { "else", SyntaxKind.ElseKeyword },
            { "elseif", SyntaxKind.ElseIfKeyword },
            { "end", SyntaxKind.EndKeyword },
            { "false", SyntaxKind.FalseKeyValue },
            { "for", SyntaxKind.ForKeyword },
            { "function", SyntaxKind.FunctionKeyword },
            { "goto", SyntaxKind.GotoKeyword },
            { "if", SyntaxKind.IfKeyword },
            { "in", SyntaxKind.InKeyword },
            { "local", SyntaxKind.LocalKeyword },
            { "nil", SyntaxKind.NilKeyValue },
            { "not",  SyntaxKind.NotUnop },
            { "or",  SyntaxKind.OrBinop },
            { "repeat", SyntaxKind.RepeatKeyword },
            { "return", SyntaxKind.ReturnKeyword },
            { "then", SyntaxKind.ThenKeyword },
            { "true", SyntaxKind.TrueKeyValue },
            { "until", SyntaxKind.UntilKeyword },
            { "while", SyntaxKind.WhileKeyword }
        };

        private static readonly IReadOnlyDictionary<string, SyntaxKind> Symbols = new Dictionary<string, SyntaxKind>
        {
            //TODO: include vararg operators
            { "-", SyntaxKind.MinusOperator }, //TODO: deal with ambiguity
            { "~", SyntaxKind.TildeUnOp }, //TODO: deal with ambiguity
            { "#", SyntaxKind.LengthUnop },
            { "~=", SyntaxKind.NotEqualsOperator },
            { "<=", SyntaxKind.LessOrEqualOperator },
            { ">=", SyntaxKind.GreaterOrEqualOperator },
            { "==", SyntaxKind.EqualityOperator },
            { "+", SyntaxKind.PlusOperator },
            { "*", SyntaxKind.MultiplyOperator },
            { "/", SyntaxKind.DivideOperator },
            { "//", SyntaxKind.FloorDivideOperator },
            { "^", SyntaxKind.ExponentOperator },
            { "%", SyntaxKind.ModulusOperator },
            { "&", SyntaxKind.BitwiseAndOperator },
            { "|", SyntaxKind.BitwiseOrOperator },
            { ">>", SyntaxKind.BitwiseRightOperator },
            { "<<", SyntaxKind.BitwiseLeftOperator },
            { "..", SyntaxKind.StringConcatOperator },
            { ">", SyntaxKind.GreaterThanOperator },
            { "<", SyntaxKind.LessThanOperator },
            { "=", SyntaxKind.AssignmentOperator },

            { "{", SyntaxKind.OpenCurlyBrace },
            { "}", SyntaxKind.CloseCurlyBrace },
            { "(", SyntaxKind.OpenParen },
            { ")", SyntaxKind.CloseParen },
            { "[", SyntaxKind.OpenBracket },
            { "]", SyntaxKind.CloseBracket },

            { ".", SyntaxKind.Dot},
            { ",", SyntaxKind.Comma},
            { ";", SyntaxKind.Semicolon},
            { ":", SyntaxKind.Colon},
            { "::", SyntaxKind.DoubleColon}
        };

        private const char Eof = unchecked((char)-1);
        private static readonly char[] longCommentID1 = { '-', '[', '[' };
        private static readonly char[] longCommentID2 = { '-', '[', '=' }; //TODO: flawed approach? what if --[=asdfadf]?

        public static List<Token> Tokenize(TextReader textReader) //TODO: Return a bool based on if this is a new copy of the lexer or not
        {
            Requires.NotNull(textReader, nameof(textReader));

            TrackableTextReader trackableTextReader = new TrackableTextReader(textReader);

            Token nextToken;
            List<Trivia> trivia;

            List<Token> tokenList = new List<Token>();

            while (!trackableTextReader.EndOfStream())
            {
                int fullStart = trackableTextReader.Position;
                trivia = ConsumeTrivia(trackableTextReader);
                nextToken = ReadNextToken(trackableTextReader, trivia, fullStart);
                tokenList.Add(nextToken);

                if (trackableTextReader.EndOfStream() && nextToken.Kind != SyntaxKind.EndOfFile)
                {
                    nextToken = new Token(SyntaxKind.EndOfFile, "", new List<Trivia>(), fullStart, trackableTextReader.Position);
                    tokenList.Add(nextToken);
                }
            }

            return tokenList;
        }
        private static List<Trivia> ConsumeTrivia(TrackableTextReader stream)
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
                            stream.Pushback();
                        }
                        break;

                    default:
                        isTrivia = false;
                        break;
                }

            } while (isTrivia);

            return triviaList;
        }

        private static Trivia ReadLongComment(TrackableTextReader stream, string commentSoFar, int? level)
        {
            // TODO: temp
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            //TODO: re-write without regex
            Regex closeBracketPattern = new Regex(@"\]={" + level.ToString() + @"}\]");

            while (!closeBracketPattern.IsMatch(commentSoFar))
            {
                commentSoFar += stream.ReadChar();
            }

            return new Trivia(Trivia.TriviaType.Comment, commentSoFar);
        }

        private static int? GetLongCommentOpenBracket(TrackableTextReader stream, ref string commentSoFar)
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


        private static Token ReadNextToken(TrackableTextReader stream, List<Trivia> trivia, int fullStart)
        {
            char nextChar;

            if (stream.EndOfStream())
            {
                return new Token(SyntaxKind.EndOfFile, "", trivia, fullStart, (int)stream.Position);
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
        private static Token ReadAlphaToken(TrackableTextReader stream, List<Trivia> trivia, int fullStart)
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
                return new Token(SyntaxKind.Identifier, value, trivia, fullStart, tokenStartPosition);
            }
        }

        private static Token ReadNumberToken(TrackableTextReader stream, List<Trivia> trivia, int fullStart)
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
                return new Token(SyntaxKind.Number, number.ToString(), trivia, fullStart, tokenStartPosition);
            }
            else
            {
                return new Token(SyntaxKind.Unknown, number.ToString(), trivia, fullStart, tokenStartPosition); //TODO: Deal with invalid number/identifier: "234kjs"
            }
        }

        private static Token ReadStringToken(char stringDelimiter, TrackableTextReader stream, List<Trivia> leadingTrivia, int fullStart)
        {
            StringBuilder fullString = new StringBuilder();
            int tokenStartPosition = (int)stream.Position;
            SyntaxKind type = SyntaxKind.String;
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
                            type = SyntaxKind.UnterminatedString;
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
                        return new Token(SyntaxKind.EndOfFile, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition); //TODO bug... should return a string then a EOF token right?
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
                            type = SyntaxKind.IgnoreNewLineString;
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

                        return new Token(SyntaxKind.UnterminatedString, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                            }
                            else
                            {
                        if (bracketLevel == 0)
                        {
                            return new Token(SyntaxKind.OpenBracket, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                        }
                        else
                        {
                                // Error, not valid syntax
                            return new Token(SyntaxKind.Unknown, fullString.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                            }
                    }
                        default:
                    throw new ArgumentOutOfRangeException(nameof(stringDelimiter), "Unrecognized String delimiter");
            }
        }

        private static Token ReadSymbolToken(TrackableTextReader stream, List<Trivia> leadingTrivia, int fullStart)
        {
            int tokenStartPosition = (int)stream.Position;
            char nextChar = stream.ReadChar();

            switch (nextChar)
            {
                case ':':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(SyntaxKind.Colon, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(SyntaxKind.DoubleColon, "::", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '.':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(SyntaxKind.Dot, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(SyntaxKind.StringConcatOperator, "..", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '<':
                    if ((nextChar != stream.Peek()) && (stream.Peek() != '='))
                    {
                        return new Token(SyntaxKind.LessThanOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
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
                        return new Token(SyntaxKind.AssignmentOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(SyntaxKind.EqualityOperator, "==", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '/':
                    if (CheckAndConsumeNextToken(nextChar, stream))
                    {
                        return new Token(SyntaxKind.DivideOperator, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(SyntaxKind.FloorDivideOperator, "//", leadingTrivia, fullStart, tokenStartPosition);
                    }
                case '~':
                    if (CheckAndConsumeNextToken('=', stream))
                    {
                        return new Token(SyntaxKind.TildeUnOp, nextChar.ToString(), leadingTrivia, fullStart, tokenStartPosition);
                    }
                    else
                    {
                        return new Token(SyntaxKind.NotEqualsOperator, "~=", leadingTrivia, fullStart, tokenStartPosition);
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
                        return new Token(SyntaxKind.Unknown, fullSymbol, leadingTrivia, fullStart, tokenStartPosition);
                    }
            }
        }

        private static bool IsAlphaCharacter(char a)
        {
            return (char.IsLetter(a) || char.IsNumber(a) || (a == '_')); //TODO? Unicode?
        }


        private static Trivia CollectWhitespace(TrackableTextReader stream)
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

        private static Trivia ReadLineComment(TrackableTextReader stream, char[] commentRead)
        {
            string comment = new string(commentRead);

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

        public static void PrintTokens(TextReader stream)
        {
            IEnumerable<Token> tokenEnumerable = Lexer.Tokenize(stream);
            foreach (Token t in tokenEnumerable)
            {
                Console.WriteLine(t);
            }
        }

        public static bool CheckAndConsumeNextToken(char character, TrackableTextReader stream)
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

        public static int CountLevels(bool validateCount, char character, int counter, TrackableTextReader stream, StringBuilder builder)
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
