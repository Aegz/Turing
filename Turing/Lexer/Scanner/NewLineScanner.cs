using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax;
using Turing.Utilities;

namespace Turing.Lexer.Scanner
{
    class NewLineScanner
    {
        public static SyntaxTrivia Scan(SlidingTextWindow TextWindow)
        {
            char xcNextChar = TextWindow.PeekCharacter();
            switch (xcNextChar)
            {
                case '\r':
                    if (TextWindow.PeekCharacter(1) == '\n')
                    {
                        TextWindow.SkipAhead(2);
                        return SyntaxFactory.EndOfLine("\r\n");
                    }

                    return SyntaxFactory.EndOfLine("\r");
                case '\n':
                    TextWindow.SkipAhead(1);
                    return SyntaxFactory.EndOfLine("\n");
                default:
                    // Try again
                    if (SyntaxUtilities.IsNewLineCharacter(xcNextChar))
                    {
                        TextWindow.SkipAhead(1);
                        return SyntaxFactory.EndOfLine("\n");
                    }

                    return null;
            }
        }
    }
}
