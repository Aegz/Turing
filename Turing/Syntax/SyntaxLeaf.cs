using System;
using Turing.Syntax.Strategies;

namespace Turing.Syntax
{
    class SyntaxLeaf : SyntaxNode
    {
        public SyntaxLeaf(SyntaxToken xoToken) : base(xoToken, NodeStrategyFactory.NULL_STRATEGY)
        {
        }

        public override bool AddChild(SyntaxNode xoGiven)
        {
            // Add no children
            return false;
        }

        public override string ToString()
        {
            switch (Token.ExpectedType)
            {
                case SyntaxKind.LiteralToken:
                    return "'" + RawSQLText + "'";

                default:
                    return base.ToString();
            }
        }
    }
}
