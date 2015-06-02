using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Expressions
{
    /// <summary>
    /// Expressions that do not have two sides (eg. NOT or an Expression surrounded by Parenthesis)
    /// </summary>
    class UnaryExpression : ExpressionSyntaxNode
    {
        public UnaryExpression(SyntaxToken xoToken) : base(xoToken, NodeStrategyFactory.UNARY_EXPRESSION_STRATEGY)
        {
           
        }
  
        public override string ToString()
        {
            switch (Token.ExpectedType)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return "(" + GetChildString() + ")";
                default:
                    return GetChildString();
            }
        }
    }
}
