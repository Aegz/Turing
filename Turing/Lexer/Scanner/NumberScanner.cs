using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;

namespace Turing.Lexer.Scanner
{
    class NumberScanner
    {
        public static Boolean ScanNumber(SlidingTextWindow TextWindow, out SyntaxToken xoToken)
        {
            // Intermediate var
            char cNextChar;
            String sCharactersProcessedSoFar = String.Empty;

            for(int iIndex = 0; ; iIndex++)
            {
                // Move to the next char
                cNextChar = TextWindow.PeekCharacter(iIndex); 

                // If the character is a valid number
                if (cNextChar >= '0' && cNextChar <= '9')
                {
                    sCharactersProcessedSoFar += cNextChar;
                }
                else if (sCharactersProcessedSoFar.Length > 0)
                {
                    xoToken = new SyntaxToken(SyntaxKind.NumericToken, sCharactersProcessedSoFar);
                    TextWindow.SkipAhead(sCharactersProcessedSoFar.Length);
                    return true;
                }
                else if (iIndex + TextWindow.Position >= TextWindow.Count)
                {
                    // Break, we couldnt find anything
                    break;
                }
            }

            // Invalid case
            xoToken = null;
            return false;
        }
    }
}
