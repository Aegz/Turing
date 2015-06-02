using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;

namespace Turing.Parser
{
    /// <summary>
    /// This class attempts to encapsulate all of the posssible resolutions to simple
    /// issues we come across during parsing. The reason why this is done during parsing
    /// is to try and facilitate continuous parsing (even when there are obvious errors).
    /// 
    /// If we do it post-parsing we may have a completely different tree
    /// </summary>
    class ResolutionGenerator
    {
        /// <summary>
        /// This only really happens with conjunctive operators (ie. +, -, AND, OR..)
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        public static void HandlePreconsumptionError (SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Its good to know whats next
            SyntaxKind eNextTokenKind = SyntaxKind.UnknownNode;

            // If we have an arithmatic operator, we know we need the types to match
            if (SyntaxKindFacts.IsArithmaticOperator(xoCurrentNode.ExpectedType) ||
                SyntaxKindFacts.IsConditionalOperator(xoCurrentNode.ExpectedType))
            {
                // 
                if (SyntaxKindFacts.IsLiteral(xoList.PeekToken().ExpectedType))
                {
                    // Set the return kind
                    eNextTokenKind = xoList.PeekToken().ExpectedType;
                }
            }
            // Conjunctive or Adjunct will not matter what types we have
            else if (SyntaxKindFacts.IsAdjunctConditionalOperator(xoCurrentNode.ExpectedType))
            {
                // MUST BE BOOLEAN
                eNextTokenKind = SyntaxKind.BooleanToken;
            }

            // Default to unknown
            xoCurrentNode.AddChild(
                new ExceptionSyntaxNode(
                    eNextTokenKind,
                    "Missing (" + SyntaxKindUtilities.GetStringFromKind(eNextTokenKind) + ")"));
        }

        /// <summary>
        /// This only really happens with conjunctive operators (ie. +, -, AND, OR..)
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        public static void HandleIncompleteNode(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Its good to know whats next
            SyntaxKind eNextTokenKind = SyntaxKind.UnknownNode;

            // If we have an arithmatic operator, we know we need the types to match
            if (SyntaxKindFacts.IsArithmaticOperator(xoCurrentNode.ExpectedType) ||
                SyntaxKindFacts.IsConditionalOperator(xoCurrentNode.ExpectedType))
            {
                // Only when we have children
                if (xoCurrentNode.Children.Count > 0)
                {
                    // Set the return kind
                    eNextTokenKind = xoCurrentNode.Children[0].ExpectedType;
                }
            }
            // Boolean
            else if (
                SyntaxKindFacts.IsAdjunctConditionalOperator(xoCurrentNode.ExpectedType) || // Conjunctive or Adjunct must have bool
                xoCurrentNode.ExpectedType == SyntaxKind.WhereKeyword || // Where must be bool
                xoCurrentNode.ExpectedType == SyntaxKind.OnKeyword  // On must be bool
                )
            {
                // MUST BE BOOLEAN
                eNextTokenKind = SyntaxKind.BooleanToken;
            }
            // Identifier
            else if (
                xoCurrentNode.ExpectedType == SyntaxKind.FromKeyword    // Table Missing
                )
            {
                // MUST BE Identifier
                eNextTokenKind = SyntaxKind.IdentifierToken;
            }

            // Default to unknown
            xoCurrentNode.AddChild(
                new ExceptionSyntaxNode(
                    eNextTokenKind,
                    "Missing (" + SyntaxKindUtilities.GetStringFromKind(eNextTokenKind) + ")"));
        }

    }
}
