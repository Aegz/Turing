using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Exceptions
{
    /// <summary>
    /// Encompasses erraneous nodes (these don't get counted)
    /// </summary>
    class SkippedNode : SyntaxNode
    {
        public SkippedNode(ISyntax xoToken) : base(xoToken)
        {
        }

    }
}
