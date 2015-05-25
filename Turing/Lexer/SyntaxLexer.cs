using System;
using System.Collections.Generic;
using System.Diagnostics;
using Turing.Lexer.Scanner;
using Turing.Syntax;
using Turing.Utilities;

namespace Turing.Lexer
{
    public class SyntaxLexer
    {
        SlidingTextWindow TextWindow;
        Boolean bHasValidSyntaxTokensLeft = true;

        public SyntaxLexer(SlidingTextWindow xoWindow)
        {
            TextWindow = xoWindow;
        }

        /// <summary>
        /// Helps determine if we can still produce tokens
        /// </summary>
        /// <returns></returns>
        public Boolean HasTokensLeftToProcess()
        {
            return TextWindow.HasCharactersLeftToProcess() && bHasValidSyntaxTokensLeft;
        }


        /// <summary>
        /// Tries to create and categorise the next available token from
        /// the given text window. If it cannot lex or there is nothing left
        /// to lex, it will return null
        /// </summary>
        /// <returns>SyntaxToken OR null</returns>
        public SyntaxToken LexNextToken()
        {
            // 1. Scan for leading Trivia
            List<SyntaxTrivia> aoLeading = LexSyntaxTrivia();

            // 2. Scan for a token
            SyntaxToken oTemp = LexSyntaxToken();

            // 3. Scan for trailing Trivia
            List<SyntaxTrivia> aoTrailing = LexSyntaxTrivia(true);
            
            // Only assign the trivia if we had something left
            if (oTemp != null && oTemp.ExpectedType != SyntaxKind.EOFToken)
            {
                // Assign the trivia
                oTemp.LeadingTrivia = aoLeading;
                oTemp.TrailingTrivia = aoTrailing;
            }
            else
            {
                bHasValidSyntaxTokensLeft = false;
            }

            // Return anything
            return oTemp;
        }


        /// <summary>
        /// Used to analyse the current textwindow and retrieve/skip any 
        /// trivia which includes things such as whitespace, newlines, 
        /// comments and multi-line comments
        /// Returns them as a flat list
        /// </summary>
        /// <param name="bIsTrailing"></param>
        /// <returns></returns>
        private List<SyntaxTrivia> LexSyntaxTrivia(Boolean bIsTrailing = false)
        {
            // Initialise variables
            List<SyntaxTrivia> aoReturnList = new List<SyntaxTrivia>();
            Boolean bNewLineCharFound = false;

            // Just keep lexing until we are told to stop
            // or we run out of characters to process
            while (TextWindow.HasCharactersLeftToProcess())
            {
                // Check the next character and switch based on that
                char xcNextChar = TextWindow.PeekCharacter();

                if (SyntaxUtilities.IsWhiteSpace(xcNextChar))
                {
                    // Try and scan the item 
                    SyntaxTrivia oNewItem = WhiteSpaceScanner.Scan(TextWindow);
                    // Insert it
                    aoReturnList.Add(oNewItem);
                }
                else if (SyntaxUtilities.IsNewLineCharacter(xcNextChar))
                {
                    bNewLineCharFound = true;
                    // Do not steal trailing comments if a newline has been found
                    if (bIsTrailing)
                    {
                        break;
                    }

                    // Try and scan the item 
                    SyntaxTrivia oNewItem = NewLineScanner.Scan(TextWindow);
                    // Insert it
                    aoReturnList.Add(oNewItem);
                }
                // Handle Comments
                else if (xcNextChar == '-')
                {
                    // Only pick up inlines if they are directly trailing an item
                    // Do not steal another nodes leading comments
                    if (bNewLineCharFound && bIsTrailing)
                    {
                        break;
                    }

                    SyntaxTrivia oComment = CommentScanner.ScanSingleLineComment(TextWindow);
                    aoReturnList.Add(oComment);
                }
                else if (xcNextChar == '/')
                {
                    // Do not steal trailing comments if a newline has been found
                    if (bNewLineCharFound && bIsTrailing)
                    {
                        break;
                    }

                    SyntaxTrivia oComment = CommentScanner.ScanMultiLineComment(TextWindow);
                    aoReturnList.Add(oComment);
                }
                else
                {
                    break;
                }
            }

            return aoReturnList;
        }
    

        /// <summary>
        /// Used to analyse the current textwindow and attempt to build
        /// proper syntax tokens which will consist of constructs such as 
        /// identifiers, keywords, operators
        /// Returns after generating a single token
        /// </summary>
        /// <returns></returns>
        private SyntaxToken LexSyntaxToken()
        {
            // Break early
            if (!TextWindow.HasCharactersLeftToProcess())
            {
                return new SyntaxToken(SyntaxKind.EOFToken, "");
            }

            // Intermediate var
            char xcNextChar = TextWindow.PeekCharacter();

            //
            switch (xcNextChar)
            {
                // Literals need to be caught first
                case '\'': // ' right case
                case '\"': // " wrong case - needs to be fixed
                    // Scan ahead to see if the literal has been closed properly
                    // ?? TODO: Create a function which can try to close off literals/comments
                    SyntaxToken oLiteralToken;
                    if (LiteralScanner.ScanLiteral(TextWindow, out oLiteralToken))
                    {
                        return oLiteralToken;
                    }
                    else
                    {
                        Debug.Write("Invalid literal token? (" + TextWindow.PopCharacter(9999) + ")");
                        // Problem.. return a dead token
                        return new SyntaxToken(
                            SyntaxKind.UnknownToken,
                            Convert.ToString(TextWindow.PopCharacter()));
                    }
                    
                // Brackets
                case '(':
                    return new SyntaxToken(
                        SyntaxKind.OpenParenthesisToken, 
                        Convert.ToString(TextWindow.PopCharacter()));
                case ')':
                    return new SyntaxToken(
                        SyntaxKind.CloseParenthesisToken, 
                        Convert.ToString(TextWindow.PopCharacter()));

                // SemiColon
                case ';':
                    return new SyntaxToken(
                        SyntaxKind.SemiColonToken,
                        Convert.ToString(TextWindow.PopCharacter()));
                
                // Equality/Comparison Operators
                case '=':
                    // Intermediate var
                    SyntaxKind eTemp = SyntaxKind.EqualsToken;

                    // If we really have an equals equals token
                    if (TextWindow.PeekCharacter(1) == '=')
                    {
                        eTemp = SyntaxKind.EqualsEqualsToken;
                    }
                    // Or an Greater than or equal token
                    else if (TextWindow.PeekCharacter(1) == '>')
                    {
                        eTemp = SyntaxKind.GreaterThanOrEqualToken;
                    }
                    // Or a less than or equal token
                    else if (TextWindow.PeekCharacter(1) == '<')
                    {
                        eTemp = SyntaxKind.LessThanOrEqualToToken;
                    }

                    // Move ahead a single char now
                    String sResult = Convert.ToString(TextWindow.PopCharacter());

                    // Move ahead again if we have a compound token
                    if (eTemp != SyntaxKind.EqualsToken)
                    {
                        sResult += TextWindow.PopCharacter();
                    }

                    // Return something
                    return new SyntaxToken(
                         eTemp,
                         sResult);
                // These two can be compound
                case '<':
                    // If we really have an equals equals token
                    if (TextWindow.PeekCharacter(1) == '=')
                    {
                        // Return something
                        return new SyntaxToken(
                             SyntaxKind.LessThanOrEqualToToken,
                             TextWindow.PopCharacter(2));
                    }
                    // If we really have an equals equals token
                    if (TextWindow.PeekCharacter(1) == '>')
                    {
                        // Return something
                        return new SyntaxToken(
                             SyntaxKind.DiamondToken,
                             TextWindow.PopCharacter(2));
                    }
                    else
                    {
                        // Return something
                        return new SyntaxToken(
                             SyntaxKind.LessThanToken,
                             Convert.ToString(TextWindow.PopCharacter()));
                    }
                case '>':
                    // If we really have an equals equals token
                    if (TextWindow.PeekCharacter(1) == '=')
                    {
                        // Return something
                        return new SyntaxToken(
                             SyntaxKind.GreaterThanOrEqualToken,
                             TextWindow.PopCharacter(2));
                    }
                    else
                    {
                        // Return something
                        return new SyntaxToken(
                             SyntaxKind.GreaterThanToken,
                             Convert.ToString(TextWindow.PopCharacter()));
                    }


                // .
                case '.':
                    // .. should be handled here in its own individual case
                    if (TextWindow.PeekCharacter(1) == '.')
                    {
                        return new SyntaxToken(
                            SyntaxKind.DotDotToken,
                            TextWindow.PopCharacter(2));
                    }
                    // Else return normal
                    return new SyntaxToken(
                        SyntaxKind.DotToken,
                        Convert.ToString(TextWindow.PopCharacter()));
                case ',':
                    return new SyntaxToken(
                        SyntaxKind.CommaToken,
                        Convert.ToString(TextWindow.PopCharacter()));

                // Hardest cases
                case '*':
                    // Did we just find an orphaned */?
                    if (TextWindow.PeekCharacter(1) == '/')
                    {
                        // ?? TODO: Figure out what to do with this
                        // This could be an issue
                        return new SyntaxToken(
                            SyntaxKind.StarSlashToken,
                            TextWindow.PopCharacter(2));
                    }

                    // Is this a star token or a multiplier?
                    return new SyntaxToken(
                        SyntaxKind.StarToken,
                        Convert.ToString(TextWindow.PopCharacter()));

                // Ampersand is allowed too (For SAS Intable)
                case '&':
                    // Invalid character (MUST TRAIL an _)
                    break;

                // Alphanumeric are possible cases for an identifier
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                    SyntaxToken oReturnToken;
                    // Scan for any keyword or identifier
                    if (IdentifierOrKeywordScanner.ScanKeywordOrIdentifier(TextWindow, out oReturnToken))
                    {
                        return oReturnToken;
                    }
                    else
                    {
                        Debug.Write("Invalid identifier token? (" + TextWindow.PeekCharacter() + ")");
                        // Problem.. return a dead token
                        return new SyntaxToken(
                            SyntaxKind.UnknownToken, 
                            Convert.ToString(TextWindow.PopCharacter()));
                    }
                
                // Number Token
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    SyntaxToken oNumberToken;
                    // ?? TODO: Scan for a Number/Integer
                    if (NumberScanner.ScanNumber(TextWindow, out oNumberToken))
                    {
                        return oNumberToken;
                    }
                    else
                    {
                        Debug.Write("Invalid numeric token? (" + TextWindow.PeekCharacter() + ")");
                        // Problem.. return a dead token
                        return new SyntaxToken(
                            SyntaxKind.UnknownToken,
                            Convert.ToString(TextWindow.PopCharacter()));
                    }

                //
                default:
                    // Try anything we possibly can here 
                    // Use the cumbersome scans to figure out what token we have
                    break;
            }

            return null;
        }


    }
}
