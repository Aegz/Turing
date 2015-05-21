using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Constructs.Keywords
{
    class OnSyntaxNode : SyntaxNode
    {
        public OnSyntaxNode(String xsRawText) : base (SyntaxKind.FromKeyword, xsRawText)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Identifiers are allowed too
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.LiteralToken },
                { SyntaxKind.NumericToken },

                // Standard Operators
                { SyntaxKind.MinusToken },
                { SyntaxKind.PlusToken },
                { SyntaxKind.StarToken },
                { SyntaxKind.SlashToken },

                // ?? TODO: Move more of these into the Expression Nodes
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
            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType)) // Table Identifiers
            {
                // Build a Column symbol
                return SymbolFactory.GenerateColumnSymbol(xoToken, xoList);
            }
            // If we come across a Expression Node 
            // Shuffle these around whie keeping Right Precedence
            else if (true)
            {
                // Shuffle the nodes around to build a series of cascading expressions
                // that follow right precedence
                return null;
            }
            else
            {
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }


    }
}
