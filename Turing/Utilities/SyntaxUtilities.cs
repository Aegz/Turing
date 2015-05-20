using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Utilities
{
    class SyntaxUtilities
    {
        /// <summary>
        /// Determines if a given character is a valid whitespace
        /// Sourced from Roselyn Compiler
        /// </summary>
        /// <param name="xcGivenChar"></param>
        /// <returns></returns>
        public static Boolean IsWhiteSpace(char xcGivenChar)
        {
            return xcGivenChar == ' '
            || xcGivenChar == '\t'
            || xcGivenChar == '\v'
            || xcGivenChar == '\f'
            || xcGivenChar == '\u00A0' // NO-BREAK SPACE
                // The native compiler, in ScanToken, recognized both the byte-order
                // marker '\uFEFF' as well as ^Z '\u001A' as whitespace, although
                // this is not to spec since neither of these are in Zs. For the
                // sake of compatibility, we recognize them both here. Note: '\uFEFF'
                // also happens to be a formatting character (class Cf), which means
                // that it is a legal non-initial identifier character. So it's
                // especially funny, because it will be whitespace UNLESS we happen
                // to be scanning an identifier or keyword, in which case it winds
                // up in the identifier or keyword.
            || xcGivenChar == '\uFEFF'
            || xcGivenChar == '\u001A'
            || (xcGivenChar > 255 && CharUnicodeInfo.GetUnicodeCategory(xcGivenChar) == UnicodeCategory.SpaceSeparator);
        }

        /// <summary>
        /// Determines if a given character is a valid whitespace
        /// Sourced from Roselyn Compiler
        /// </summary>
        /// <param name="xcGivenChar"></param>
        /// <returns></returns>
        public static Boolean IsNewLineCharacter(char xcGivenChar)
        {
            return xcGivenChar == '\r'     // Carriage Return    
            || xcGivenChar == '\n'         // Line-feed
            ;
        }


    }
}
