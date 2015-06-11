using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Turing.Lexer;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Exceptions;
using Turing.Syntax.Strategies;

namespace Turing.Parser
{
    /// <summary>
    /// This class attempts to encapsulate all of the posssible resolutions to simplify
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
                if (SyntaxKindFacts.IsLiteral(xoContext.List.Peek().ExpectedType))
                {
                    // Set the return kind
                    eNextTokenKind = xoContext.List.Peek().ExpectedType;
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

        /// <summary>
        /// If a comment contains stuff that we need (ie. Accidentally
        /// introducing a single line comment that is not terminated)
        /// </summary>
        /// <param name="xoContext"></param>
        /// <returns></returns>
        public static Boolean ScanSurroundingTriviaForKeyword(
            ParsingContext xoContext)
        {
            //SyntaxNode oCurrent = xoContext.CurrentNode;
            SyntaxNode oLastChild = xoContext.CurrentNode.GetLastChild();
            SyntaxToken oNextToken = (SyntaxToken)xoContext.List.Peek();

            // Scan the trailing trivia of the last node created 
            SyntaxTrivia oTrailingTrivia = oLastChild.Token.TrailingTrivia.Find((oTrivia) => 
                oTrivia.ExpectedType == SyntaxKind.MultiLineCommentTrivia ||
                oTrivia.ExpectedType == SyntaxKind.SingleLineCommentTrivia);

            List<ISyntax> xaoNewTokens;

            // Use the leading trivia of the next node
            if (ScanTriviaForKeywords(xoContext, oTrailingTrivia, out xaoNewTokens))
            {
                // Add the tokens
                xoContext.List.Insert(xaoNewTokens, xoContext.List.Position);
                return true;
            }

            // Try leading trivia
            SyntaxTrivia oLeadingTrivia = oNextToken.LeadingTrivia.Find((oTrivia) =>
                oTrivia.ExpectedType == SyntaxKind.MultiLineCommentTrivia ||
                oTrivia.ExpectedType == SyntaxKind.SingleLineCommentTrivia);

            // Use the leading trivia of the next node
            if (ScanTriviaForKeywords(xoContext, oLeadingTrivia, out xaoNewTokens))
            {
                // Add the tokens
                xoContext.List.Insert(xaoNewTokens, xoContext.List.Position);
                return true;
            }

            // Scan Trailing of last

            return false;
        }

        /// <summary>
        /// This is very unsafe as everything is converted to Upper
        /// before the conversion (ie. it matches against any case)
        /// </summary>
        /// <param name="xoTrivia"></param>
        /// <returns></returns>
        private static Boolean ScanTriviaForKeywords(
            ParsingContext xoContext,
            SyntaxTrivia xoTrivia,
            out List<ISyntax> xaoReturnList)
        {
            // Initialise the out var
            xaoReturnList = new List<ISyntax>();

            // Exit early if invalid argument passed
            if (xoTrivia == null)
            {
                return false;
            }

            // Break apart the trivia text
            String[] asTriviaTextTokens = xoTrivia.RawSQLText.Split();

            // Start looping through the trivia text
            for (int iIndex = 0; iIndex < asTriviaTextTokens.Length; iIndex++)
            {
                // For each string in the Comment/Trivia
                String sLoopingVar = asTriviaTextTokens[iIndex];
                SyntaxKind ePossibleKind = SyntaxKindUtilities.GetKindFromString(sLoopingVar); // Try and get a kind

                // If we have a positive match
                if (ePossibleKind != SyntaxKind.UnknownNode)
                {
                    // Mayfly
                    SyntaxToken oToken = new SyntaxToken(ePossibleKind, sLoopingVar);

                    // If we can consume this node (it is something we expect)
                    if (xoContext.CurrentNode.CanConsumeNode(
                            new ParsingContext(xoContext.CurrentNode, 
                            new SyntaxList(oToken)), 
                            false) == CanConsumeResult.Consume) 
                    {
                        // Create tokens from everything beyond this Keyword we can use (because they will most likely be
                        // a part of the new keyword)
                        String sRemainingText = String.Join(" ", asTriviaTextTokens.Skip(iIndex));
                        xoTrivia.RawSQLText = String.Join(" ", asTriviaTextTokens.Take(iIndex));

                        // Start lexing the rest of the terms
                        SyntaxLexer oLexer = new SyntaxLexer(new SlidingTextWindow(sRemainingText));

                        // Add the new tokens to our list
                        while (oLexer.HasTokensLeftToProcess())
                        {
                            xaoReturnList.Add(oLexer.LexNextToken());
                        }
                    }
                }
            }

            return xaoReturnList.Count > 0;
        }

    }
}
