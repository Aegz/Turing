using System.Collections.Generic;
using Turing.Factories;
using Turing.Syntax.Collections;

namespace Turing.Syntax.Constructs.Keywords
{
    class OnSyntaxNode : SyntaxNode
    {
        public OnSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            ConsumableTypes.AddRange(new List<SyntaxKind>
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

                { SyntaxKind.CaseKeyword },
                { SyntaxKind.AndKeyword },
                { SyntaxKind.OrKeyword },
            });
        }

        public override SyntaxNode ConvertTokenIntoNode(SyntaxTokenList xoList)
        {
            // If we have an identifier on its own
            if (SyntaxNode.IsIdentifier(xoList.PeekToken().ExpectedType))
            {
                return SymbolFactory.GenerateColumnSymbol(xoList);
            }
            else
            {
                // Let the base conversion figure out what it is
                return base.ConvertTokenIntoNode(xoList);
            }
        }


    }
}
