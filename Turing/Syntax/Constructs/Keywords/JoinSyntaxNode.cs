using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Constructs.Keywords
{
    class JoinSyntaxNode : SyntaxNode
    {
        public JoinSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // On keywords
                { SyntaxKind.OnKeyword },

                // Identifiers are allowed too
                { SyntaxKind.IdentifierToken },
            });

            // Assign an eligibility function
            ConsumptionEligibilityFn = PreviousChildIsEligible;
        }

        private Boolean PreviousChildIsEligible(SyntaxNode xoNode)
        {
            return 
                    xoNode.ExpectedType == SyntaxKind.IdentifierToken || // Identifier/(Table)
                    xoNode.ExpectedType == SyntaxKind.JoinKeyword || // Or a previous Join Structure
                    SyntaxNode.IsJoinTypeKeyword(xoNode.ExpectedType); // Join type keyword
        }

        public override bool TryConsumeList(SyntaxTokenList xoWindow)
        {
            // Consume the previous sibling before consuming anything else
            if (TryConsumePreviousSibling())
            {
                String FoundType = Parent.Children.Count > 0 ? Parent.Children[Parent.Children.Count - 1].RawSQLText : "NONE";

                // ERROR? Nothing to consume?
                Comments.Add(new StatusItem(
                    String.Format(
                        ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE,
                        "Identifier or Join", FoundType
                    )));
            }

            return base.TryConsumeList(xoWindow);
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
