using System;
using System.Diagnostics;
using Turing.Syntax;

namespace Turing.Lexer.Scanner
{
    class IdentifierOrKeywordScanner
    {

        public static Boolean ScanKeywordOrIdentifier(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            // Found some valid identifier
            if (ScanIdentifier(TextWindow, out xoToken))
            {
                SyntaxKind eNewType = SyntaxKindConverter.ConvertKeywordIntoSyntaxKind(xoToken.RawSQLText);

                // If it is not a core keyword then presume its an identifier
                xoToken.ExpectedType = eNewType != SyntaxKind.UnknownNode ? eNewType : SyntaxKind.IdentifierToken;

                return true;
            }

            // No identifier could be generated
            return false;
        }
        
        /// <summary>
        /// Try and scan for an identifier
        /// </summary>
        /// <param name="TextWindow"></param>
        /// <param name="xoToken"></param>
        /// <returns></returns>
        private static Boolean ScanIdentifier(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            // If the fast scan succeeds
            if (FastIdentifierScan(TextWindow, out xoToken))
            {
                return true;
            }
            else
            {
                // ?? TODO: Implement slow scan
                Debug.Write("Need a Slow Scan");
                // Try the slow scan
                return FastIdentifierScan(TextWindow, out xoToken);
            }
        }

        /// <summary>
        /// Simple scan ahead for valid characters that ends as soon as
        /// an invalid character comes up.
        /// 
        /// If it doesn't finish/reach a valid ending then it fails
        /// </summary>
        /// <param name="TextWindow"></param>
        /// <param name="xoToken"></param>
        /// <returns></returns>
        private static Boolean FastIdentifierScan(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            // Initialise a return string
            String sCharactersScannedSoFar = String.Empty;

            // 
            for (int iIndex = 0; TextWindow.Position + iIndex < TextWindow.Count; iIndex++)
            {
                // 
                char cNextChar = TextWindow.PeekCharacter(iIndex);
                
                switch (cNextChar)
                {
                    // Any invalid character that doesn't belong in an identifier
                    case '\0':
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                    case '!':
                    case '%':
                    case '(':
                    case ')':
                    case '*':
                    case '+':
                    case ',':
                    case '-':
                    case '.':
                    case '/':
                    case ':':
                    case ';':
                    case '<':
                    case '=':
                    case '>':
                    case '?':
                    case '[':
                    case ']':
                    case '^':
                    case '{':
                    case '|':
                    case '}':
                    case '~':
                    case '"':
                    case '\'':
                        // Closing case (anything that is not valid in an identifier)
                        xoToken = new SyntaxToken(SyntaxKind.UnknownNode, sCharactersScannedSoFar);
                        // Skip ahead the length of the token
                        TextWindow.SkipAhead(xoToken.RawSQLText.Length);
                        // Is true if we had a valid token (string)
                        return sCharactersScannedSoFar.Length > 0;

                    case '&':
                        // This MUST trail an _
                        if (iIndex > 0 && sCharactersScannedSoFar[sCharactersScannedSoFar.Length -1] == '_')
                        {
                            // Closing case (anything that is not valid in an identifier)
                            xoToken = new SyntaxToken(SyntaxKind.UnknownNode, sCharactersScannedSoFar);
                            // Skip ahead the length of the token
                            TextWindow.SkipAhead(xoToken.RawSQLText.Length);
                            // Is true if we had a valid token (string)
                            return sCharactersScannedSoFar.Length > 0;
                        }
                        // Keep going
                        break;

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
                        // Numbers cannot lead
                        if (iIndex != 0)
                        {
                            // Ignore and add to the characters scanned so far
                            break;
                        }
                        else
                        {
                            // We have come across an invalid token?
                            // A number is trying to be identified as an identifier
                            xoToken = null;
                            return false;
                        }
                    
                    // Valid cases
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
                        // Do nothing and just add the character to the string?
                        break;
                    // No idea what has come here..
                    default:

                        break;
                }

                // Add characters after looping
                sCharactersScannedSoFar += cNextChar;
            }

            // Could not identify anything valid
            xoToken = null;
            return false;
        }

        // ?? TODO: Implement this if necessary
        private static Boolean SlowIdentifierScan(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            xoToken = null;
            return false;
        }
    }
}
