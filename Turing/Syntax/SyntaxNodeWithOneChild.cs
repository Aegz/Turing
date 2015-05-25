using System;

namespace Turing.Syntax
{
    /// <summary>
    /// A syntax node that can only have a single child. Examples of where this might
    /// be applicable include use of the Symbol
    /// </summary>
    class SyntaxNodeWithOneChild : SyntaxNode
    {
        public SyntaxNodeWithOneChild (SyntaxToken xoToken) : base(xoToken)
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
