using System.Collections.Generic;
using Turing.Factories;
using Turing.Syntax.Collections;

namespace Turing.Syntax.Constructs.Keywords
{
    class OnSyntaxNode : SyntaxNode
    {
        public OnSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Identifiers are allowed too
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.LiteralToken },
                { SyntaxKind.NumericToken },

                // MOVE TO Expression?

                // Standard Operators
                { SyntaxKind.MinusToken },
                { SyntaxKind.PlusToken },
                { SyntaxKind.StarToken },
                { SyntaxKind.SlashToken },

                // Conditional Operators
                { SyntaxKind.EqualsToken },
                { SyntaxKind.InKeyword },
                { SyntaxKind.IsKeyword },
                { SyntaxKind.NotKeyword },
                { SyntaxKind.NullKeyword },

                // Compound Operators
                { SyntaxKind.GreaterThanOrEqualToken },
                { SyntaxKind.GreaterThanToken },
                { SyntaxKind.LessThanOrEqualToToken },
                { SyntaxKind.LessThanToken },

            });
        }

        public override SyntaxNode ConvertTokenIntoNode(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // Handle the more unique cases first
            // We start the condition with a NOT (entirely possible)
            if (xoToken.ExpectedType == SyntaxKind.NotKeyword)
            {
                // Then we should build an expression out
                
            }
            // If we have an identifier followed by an operator of some sort
            else if (SyntaxNode.IsIdentifier(xoToken.ExpectedType) || SyntaxNode.IsOperator(xoToken.ExpectedType))
            {

            }

            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType)) // Table Identifiers
            {
                // Build a Column symbol
                return SymbolFactory.GenerateColumnSymbol(xoToken, xoList);
            }
            // If we come across a Expression Node 
            // Shuffle these around whie keeping Right Precedence
            //else if (true)
            //{
            //    // Shuffle the nodes around to build a series of cascading expressions
            //    // that follow right precedence
            //    return null;
            //}
            else
            {
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }


    }
}
