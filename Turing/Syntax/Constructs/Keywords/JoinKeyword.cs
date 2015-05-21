using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Constructs.Keywords
{
    class JoinKeyword : SyntaxNode
    {
        public JoinKeyword(String xsRawText) : base(SyntaxKind.FromKeyword, xsRawText)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // On keywords
                { SyntaxKind.OnKeyword },

                // Identifiers are allowed too
                { SyntaxKind.IdentifierToken },
            });
        }

        public override SyntaxNode ConvertTokenIntoNode(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType)) // Table Identifiers
            {
                // Build a table symbol if necessary
                return SymbolFactory.GenerateTableSymbol(xoToken, xoList);
            }
            else
            {
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }
    }
}
