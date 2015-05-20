using System;
using System.Collections.Generic;
using Turing.Syntax;

namespace Turing.Lexer.Scanner
{
    class LiteralScanner
    {
        public static Boolean ScanLiteral(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            // Initialise a return string
            String sCharacterScannedSoFar = String.Empty;

            // 
            for (int iIndex = 1; TextWindow.Position + iIndex < TextWindow.Count; iIndex++)
            {
                // 
                char cNextChar = TextWindow.PeekCharacter(iIndex);
                
                // If we find an ending character
                if (cNextChar == '\'' || cNextChar == '\"')
                {
                    xoToken = new SyntaxToken(SyntaxKind.LiteralToken, sCharacterScannedSoFar);
                    // Plus 2 to skip passed the quotation marks
                    TextWindow.SkipAhead(sCharacterScannedSoFar.Length + 2);
                    return true;
                }

                // Add the character after
                sCharacterScannedSoFar += cNextChar;
            }

            // If we reach here, no literal could be built.
            xoToken = null;
            return false;
        }
    }
}
