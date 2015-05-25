using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Keywords
{
    class WhereSyntaxNode : SyntaxNode
    {
        public WhereSyntaxNode(SyntaxToken xoToken) : base (xoToken)
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

                { SyntaxKind.CaseKeyword },
                { SyntaxKind.AndKeyword },
                { SyntaxKind.OrKeyword },

            });
        }

    }
}
