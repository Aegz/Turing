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
    class ExceptionSyntaxNode : SyntaxNode
    {
        public ExceptionSyntaxNode() : base (new SyntaxToken(SyntaxKind.UnknownToken, "?MISSING?"))
        {
        }

        public ExceptionSyntaxNode(String xsReason) : base (new SyntaxToken(SyntaxKind.UnknownToken, xsReason))
        {
        }
    }
}
