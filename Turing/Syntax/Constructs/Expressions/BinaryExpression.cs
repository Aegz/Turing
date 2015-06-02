using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Expressions
{
    /// <summary>
    /// A Binary Expression Object
    /// </summary>
    class BinaryExpression : ExpressionSyntaxNode
    {
        public BinaryExpression(SyntaxToken xoToken) : base(xoToken, NodeStrategyFactory.BINARY_EXPRESSION_STRATEGY)
        {
        }

        public override string ToString()
        {
            return GetChildString();
        }


        public override string GetChildString()
        {
            return String.Format(
                "{0} {1} {2}", 
                (Children.Count > 0 ? Children[0].ToString() : ""), 
                RawSQLText,
                (Children.Count > 1 ? Children[1].ToString() : ""));
        }
    }
}
