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
        public ExceptionSyntaxNode(String xsRawText) : base(new SyntaxToken(SyntaxKind.MissingNode, xsRawText))
        {
        }

        public ExceptionSyntaxNode() : this("")
        {
        }
    }
}
