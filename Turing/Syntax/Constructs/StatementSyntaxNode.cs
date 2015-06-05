using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs
{
    public class StatementSyntaxNode : SyntaxNode
    {
        public StatementSyntaxNode() : base(SyntaxToken.NULL_TOKEN)
        {
            // Always start with 1
            //Children.Add(new StatementSyntaxNode());

        }

    }
}
