using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Keywords
{
    // ?? TODO: Roll this into a strategy that has a fn for consuming previous children
    public class JoinSyntaxNode : SyntaxNode
    {
        public JoinSyntaxNode(SyntaxToken xoToken) : base(xoToken, NodeStrategyFactory.JOIN_STRATEGY)
        {
        }

        /// <summary>
        /// Determines which nodes are consumeable
        /// </summary>
        /// <param name="xoNode"></param>
        /// <returns></returns>
        protected override Boolean PreviousChildIsEligible(SyntaxNode xoNode)
        {
            return
                    SyntaxKindFacts.IsIdentifier(SyntaxKind.IdentifierToken) || // Identifier/(Table)
                    SyntaxKindFacts.IsJoinKeyword(xoNode.ExpectedType); // Join type keyword
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xoWindow"></param>
        /// <returns></returns>
        public override bool TryConsumeList(SyntaxTokenList xoWindow)
        {
            // Consume the previous sibling before consuming anything else
            if (!TryConsumePreviousSibling((oKind) =>
                    SyntaxKindFacts.IsIdentifier(oKind) || // Identifier/(Table)
                    SyntaxKindFacts.IsJoinKeyword(oKind)))
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


        public override string ToString()
        {
            return GetChildString();
        }

        public override string GetChildString()
        {
            return String.Format(
                "{0} {1} {2} {3}", //ON
                (Children.Count > 0 ? Children[0].ToString() : ""),
                RawSQLText,
                (Children.Count > 1 ? Children[1].ToString() : ""),
                (Children.Count > 2 ? Children[2].ToString() : ""));
        }
    }
}
