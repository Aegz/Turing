using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;
using Turing.Factories;
using Turing.Utilities;

namespace Turing.Lexer.Scanner
{
    class WhiteSpaceScanner 
    {
        /// <summary>
        /// Scans through the window and generates a WhiteSpace SyntaxNode/Trivia
        /// </summary>
        /// <param name="TextWindow"></param>
        /// <returns></returns>
        public static SyntaxTrivia Scan(SlidingTextWindow TextWindow)
        {
            // Default to OnlyWhiteSpace true
            Boolean bIsOnlySpaces = true;
            Boolean bEndOfTrivia = false;
            String sCharactersScannedSoFar = String.Empty;

            while (!bEndOfTrivia && TextWindow.HasCharactersLeftToProcess())
            {
                // Peek at a character
                char ch = TextWindow.PeekCharacter();

                switch (ch)
                {
                    case '\t':       // Horizontal tab
                    case '\v':       // Vertical Tab
                    case '\f':       // Form-feed
                    case '\u001A':
                        bIsOnlySpaces = false;
                        goto case ' ';
                    case ' ':
                        sCharactersScannedSoFar += TextWindow.PopCharacter();
                        break;

                    case '\r':      // Carriage Return
                    case '\n':      // Line-feed
                        // End iteration
                        bEndOfTrivia = true;
                        break;

                    default:
                        if (ch > 127 && SyntaxUtilities.IsWhiteSpace(ch))
                        {
                            goto case '\t';
                        }
                        bEndOfTrivia = true;
                        break;
                }
            }

            // Return the specific instance where possible
            if (sCharactersScannedSoFar.Length == 0)
            {
                return null;
            }
            if (bIsOnlySpaces && sCharactersScannedSoFar.Length == 1)
            {
                return SyntaxFactory.Space;
            }
            else
            {
                return SyntaxFactory.WhiteSpace(sCharactersScannedSoFar);
            }
        }

    }
}
