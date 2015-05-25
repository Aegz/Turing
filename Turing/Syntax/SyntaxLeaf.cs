using System;

namespace Turing.Syntax
{
    class SyntaxLeaf : SyntaxNode
    {
        public SyntaxLeaf(SyntaxToken xoToken) : base(xoToken)
        {
        }

        public override bool AddChild(SyntaxNode xoGiven)
        {
            // Add no children
            return false;
        }
    }
}
