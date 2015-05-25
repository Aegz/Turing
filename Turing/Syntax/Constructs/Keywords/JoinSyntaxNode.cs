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
    public class JoinSyntaxNode : SyntaxNode
    {
        public Boolean IsOuter = false;
       
        public JoinSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            ConsumableTypes.AddRange(new List<SyntaxKind>
            {
                // On keywords
                { SyntaxKind.OnKeyword },

                // Identifiers are allowed too
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.OpenParenthesisToken },
            });

        }

        /// <summary>
        /// Determines which nodes are consumeable
        /// </summary>
        /// <param name="xoNode"></param>
        /// <returns></returns>
        protected override Boolean PreviousChildIsEligible(SyntaxNode xoNode)
        {
            return 
                    xoNode.ExpectedType == SyntaxKind.IdentifierToken || // Identifier/(Table)
                    xoNode.ExpectedType == SyntaxKind.JoinKeyword || // Or a previous Join Structure
                    SyntaxNode.IsJoinTypeKeyword(xoNode.ExpectedType); // Join type keyword
        }


        public override bool TryConsumeList(SyntaxTokenList xoWindow)
        {
            // Consume the previous sibling before consuming anything else
            if (!TryConsumePreviousSibling())
            {
                String FoundType = Parent.Children.Count > 0 ? Parent.Children.LastOrDefault(PreviousChildIsEligible).RawSQLText : "NONE";
          
                // ERROR? Nothing to consume?
                InsertStatusMessage(
                    String.Format(
                        ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE,
                        "Identifier or Join", FoundType
                    ));
            }

            return base.TryConsumeList(xoWindow);
        }


        public override SyntaxNode ConvertTokenIntoNode(SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();
            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoCurrentToken.ExpectedType) ||         // Generic Identifiers only
                xoCurrentToken.ExpectedType == SyntaxKind.OpenParenthesisToken) // Subqueries
            {
                // Build a table symbol if necessary
                return SymbolFactory.GenerateTableSymbol(xoList);
            }
            else
            {
                return base.ConvertTokenIntoNode(xoList);
            }
        }

        public override string ToString()
        {
            return GetChildString();
        }

        public override string GetChildString()
        {
            return String.Format(
                "{0} {1} {2}",
                (Children.Count > 0 ? Children[0].ToString() : ""),
                RawSQLText,
                (Children.Count > 1 ? Children[1].ToString() : ""));
        }
    }
}
