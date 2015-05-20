using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs
{
    class QuerySyntaxNode : SyntaxNode
    {
        public QuerySyntaxNode() : base()
        {
            // Always start with 1
            Children.Add(new StatementSyntaxNode());

        }

        public QuerySyntaxNode(List<SyntaxNode> xaoChildren) : base()
        {
            aoChildren = xaoChildren;
        }

        public override bool TryAddChild(SyntaxNode xoNode)
        {
            // Check that we are inserting the right type
            if (xoNode.GetType() == typeof(StatementSyntaxNode)) // Only accept statements
            {
                return base.TryAddChild(xoNode);
            }

            return false;
        }

    }
}
