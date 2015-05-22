using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Parser;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.SingleChild;

namespace Turing.Syntax.Constructs.Keywords
{
    class SelectSyntaxNode : SyntaxNode
    {
        public SelectSyntaxNode(SyntaxToken xoToken) : base (xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Star and Columns
                { SyntaxKind.StarToken },
                { SyntaxKind.IdentifierToken },

                // Core Keywords
                { SyntaxKind.FromKeyword },
                { SyntaxKind.WhereKeyword },
                { SyntaxKind.GroupByKeyword },
                { SyntaxKind.OrderByKeyword },
                { SyntaxKind.LimitKeyword },
            });
        }

        public override SyntaxNode ConvertTokenIntoNode(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType) || // Generic Identifiers allowed here too
                xoToken.ExpectedType == SyntaxKind.StarToken) // * in Column is allowed
            {
                // Perform context sensitive conversion here
                return new ColumnSymbol(xoToken);
            }

            // Default to using the original conversion
            return base.ConvertTokenIntoNode(xoToken, xoList);

        }
    }
}
