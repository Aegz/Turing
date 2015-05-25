using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs
{
    public class StatementSyntaxNode : SyntaxNode
    {
        public StatementSyntaxNode() : base()
        {
            // Always start with 1
            //Children.Add(new StatementSyntaxNode());

        }

        public StatementSyntaxNode(List<SyntaxNode> xaoChildren) : base()
        {
            aoChildren = xaoChildren;
        }

        public override bool AddChild(SyntaxNode xoGiven)
        {
            if (xoGiven.Children.Count == 0)
            {
                return false;
            }
            return base.AddChild(xoGiven);
        }
    }
}
