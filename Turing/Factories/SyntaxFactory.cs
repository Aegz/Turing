using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;

namespace Turing.Factories
{
    class SyntaxFactory
    {
        #region Common Trivia
        // Hardcoded static instances
        public static readonly SyntaxTrivia Space = new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");
        public static readonly SyntaxTrivia EmptyChar = new SyntaxTrivia(SyntaxKind.EmptyCharTrivia, "");

        #endregion

        #region Common Tokens

        public static readonly SyntaxToken Plus = new SyntaxToken(SyntaxKind.PlusToken, "+");

        #endregion

        // Construction Methods
        public static SyntaxTrivia WhiteSpace(String xsText = " ")
        {
            return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, xsText);
        }

        public static SyntaxTrivia EndOfLine(String xsText = " ")
        {
            return new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, xsText);
        }

        public static SyntaxTrivia SingleLineComment(String xsText = " ")
        {
            return new SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, xsText);
        }

        public static SyntaxTrivia MultiLineComment(String xsText = " ")
        {
            return new SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, xsText);
        }


    }
}
