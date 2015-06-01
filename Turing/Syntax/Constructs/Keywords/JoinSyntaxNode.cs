using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Keywords
{
    // ?? TODO: Roll this into a strategy that has a fn for consuming previous children
    public class JoinSyntaxNode : SyntaxNode
    {
        public JoinSyntaxNode(SyntaxToken xoToken) : base(xoToken, NodeStrategyFactory.JOIN_STRATEGY)
        {
        }


        public override string ToString()
        {
            return GetChildString();
        }

        public override string GetChildString()
        {
            return String.Format(
                "{0} {1} {2} {3}", //ON
                (Children.Count > 0 ? Children[0].ToString() : ""),
                RawSQLText,
                (Children.Count > 1 ? Children[1].ToString() : ""),
                (Children.Count > 2 ? Children[2].ToString() : ""));
        }
    }
}
