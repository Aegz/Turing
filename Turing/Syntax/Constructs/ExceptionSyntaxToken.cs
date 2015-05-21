using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs
{
    /// <summary>
    /// Encompasses missing or erraneous nodes
    /// </summary>
    class ExceptionSyntaxToken : SyntaxToken
    {
        public ExceptionSyntaxToken() : base (SyntaxKind.UnknownToken, "?")
        {
        }
    }
}
