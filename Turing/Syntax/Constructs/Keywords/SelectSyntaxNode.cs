using System;
using System.Collections.Generic;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.Collections;
using Turing.Syntax.Constructs.Symbols.SingleChild;

namespace Turing.Syntax.Constructs.Keywords
{
    public class SelectSyntaxNode : SyntaxNode
    {
        public SelectSyntaxNode(SyntaxToken xoToken) : base (xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Star and Columns
                { SyntaxKind.StarToken },
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.CommaToken },
                { SyntaxKind.CaseKeyword }, // Case is unfortunately something that works in columns
                //{ SyntaxKind.AsKeyword },

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
            // Build a Symbol Composite

            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType) || // Generic Identifiers allowed here too
                xoToken.ExpectedType == SyntaxKind.StarToken) // * in Column is allowed
            {
                // Initialise a list
                SymbolList oList = new SymbolList();

                // Generate the column
                ColumnSymbol oColumn = new ColumnSymbol(xoToken);

                // Scan ahead for an alias to attach
                oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                // If Alias was found for a *
                if (xoToken.ExpectedType == SyntaxKind.StarToken)
                {
                    // Perform context sensitive conversion here
                    oColumn.Alias = String.Empty; // Set it back to null
                }

                // Add this column
                oList.AddChild(oColumn);

                // generate a symbol list (which will consume anything else that is a column)
                return oList;
            }

            // Default to using the original conversion
            return base.ConvertTokenIntoNode(xoToken, xoList);
        }
    }
}
