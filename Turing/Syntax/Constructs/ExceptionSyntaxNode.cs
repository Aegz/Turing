using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs
{
    /// <summary>
    /// Encompasses missing or erraneous nodes
    /// </summary>
    class ExceptionSyntaxNode : SyntaxNode
    {
        public ExceptionSyntaxNode(SyntaxKind xeKind, String xsRawText) : base(new SyntaxToken(xeKind, xsRawText))
        {
        }

        public ExceptionSyntaxNode(String xsRawText) : this(SyntaxKind.MissingNode, xsRawText)
        {
        }

        public ExceptionSyntaxNode() : this("")
        {
        }
    }
}
