using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Expressions
{
    class BooleanExpressionSyntaxNode : ExpressionSyntaxNode
    {
        public BooleanExpressionSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Identifier are allowed 
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.BooleanToken },
                { SyntaxKind.NumericToken },
                { SyntaxKind.LiteralToken },
                { SyntaxKind.NullKeyword },

                // Basic Operators
                { SyntaxKind.PlusToken },
                { SyntaxKind.MinusToken },
                { SyntaxKind.StarToken },
                { SyntaxKind.SlashToken },

                // Unique Comparison Operators
                { SyntaxKind.InKeyword },
                { SyntaxKind.IsKeyword },
                { SyntaxKind.NotKeyword },
                { SyntaxKind.NullKeyword },

                // Core Comparison Operators
                { SyntaxKind.DiamondToken },
                { SyntaxKind.GreaterThanOrEqualToken },
                { SyntaxKind.GreaterThanToken },
                { SyntaxKind.LessThanOrEqualToToken },
                { SyntaxKind.LessThanToken },
                { SyntaxKind.EqualsEqualsToken },
                { SyntaxKind.EqualsToken },

                // CASE (will be allowed in expressions
                { SyntaxKind.CaseKeyword }
            });
        }

        
    }
}
