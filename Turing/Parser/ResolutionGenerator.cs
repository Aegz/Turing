using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Exceptions;

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
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        public static void HandlePreconsumptionError(ParsingContext xoContext)
        {
            // Its good to know whats next
            SyntaxKind eNextTokenKind = SyntaxKind.UnknownNode;

            // If we have an arithmatic operator, we know we need the types to match
            if (SyntaxKindFacts.IsArithmaticOperator(xoContext.CurrentNode.ExpectedType) ||
                SyntaxKindFacts.IsConditionalOperator(xoContext.CurrentNode.ExpectedType))
            {
                // 
                if (SyntaxKindFacts.IsLiteral(xoContext.List.PeekToken().ExpectedType))
                {
                    // Set the return kind
                    eNextTokenKind = xoContext.List.PeekToken().ExpectedType;
                }
            }
            // Conjunctive or Adjunct will not matter what types we have
            else if (SyntaxKindFacts.IsAdjunctConditionalOperator(xoContext.CurrentNode.ExpectedType))
            {
                // MUST BE BOOLEAN
                eNextTokenKind = SyntaxKind.BooleanToken;
            }

            // Default to unknown Filler
            xoContext.CurrentNode.Add(
                new FillerNode(
                    eNextTokenKind,
                    "Missing (" + SyntaxKindUtilities.GetStringFromKind(eNextTokenKind) + ")"));
        }

        /// <summary>
        /// This only really happens with conjunctive operators (ie. +, -, AND, OR..)
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        public static void HandleIncompleteNode(ParsingContext xoContext)
        {
            // Its good to know whats next
            SyntaxKind eNextTokenKind = SyntaxKind.UnknownNode;

            // If we have an arithmatic operator, we know we need the types to match
            if (SyntaxKindFacts.IsArithmaticOperator(xoContext.CurrentNode.ExpectedType) ||
                SyntaxKindFacts.IsConditionalOperator(xoContext.CurrentNode.ExpectedType))
            {
                // Only when we have children
                if (xoContext.CurrentNode.Count > 0)
                {
                    // Set the return kind
                    eNextTokenKind = xoContext.CurrentNode[0].ExpectedType;
                }
            }
            // Boolean
            else if (
                SyntaxKindFacts.IsAdjunctConditionalOperator(xoContext.CurrentNode.ExpectedType) || // Conjunctive or Adjunct must have bool
                xoContext.CurrentNode.ExpectedType == SyntaxKind.WhereKeyword || // Where must be bool
                xoContext.CurrentNode.ExpectedType == SyntaxKind.OnKeyword  // On must be bool
                )
            {
                // MUST BE BOOLEAN
                eNextTokenKind = SyntaxKind.BooleanToken;
            }
            // Identifier
            else if (
                xoContext.CurrentNode.ExpectedType == SyntaxKind.FromKeyword    // Table Missing
                )
            {
                // MUST BE Identifier
                eNextTokenKind = SyntaxKind.IdentifierToken;
            }

            // Default to unknown
            xoContext.CurrentNode.Add(
                new FillerNode(
                    eNextTokenKind,
                    "Missing (" + SyntaxKindUtilities.GetStringFromKind(eNextTokenKind) + ")"));
        }



    }
}
