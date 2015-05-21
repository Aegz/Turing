using System;

namespace Turing.Syntax
{
    class SyntaxNodeWithOneChild : SyntaxNode
    {
        public SyntaxNodeWithOneChild (SyntaxKind xeKind, String xsText) : base (xeKind, xsText)
        {
        }

        public override bool AddChild(SyntaxNode xoGiven)
        {
            // Exit if we cant add it
            if (Children.Count == 1)
            {
                return false;
            }
            // Add it
            return base.AddChild(xoGiven);
        }
    }
}
