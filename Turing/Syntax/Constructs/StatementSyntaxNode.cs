using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs
{
    class StatementSyntaxNode : SyntaxNode
    {


        public override bool TryAddChild(SyntaxNode xoNode)
        {
            // Check that we are inserting the right type
            if (xoNode.GetType() == typeof(SyntaxNode)) // Keywords
            {
                return base.TryAddChild(xoNode);
            }
            return false;
        }
    }
}
