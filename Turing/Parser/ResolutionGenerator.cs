using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Lexer;
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

        public static Boolean ScanSurroundingTriviaForKeyword(ParsingContext xoContext)
        {
            //SyntaxNode oCurrent = xoContext.CurrentNode;
            SyntaxNode oLastChild = xoContext.CurrentNode.GetLastChild();
            SyntaxToken oNextToken = xoContext.List.PeekToken();

            // Scan the trailing trivia of the last node created 
            SyntaxTrivia oTrailingTrivia = oLastChild.Token.TrailingTrivia.Find((oTrivia) => 
                oTrivia.ExpectedType == SyntaxKind.MultiLineCommentTrivia ||
                oTrivia.ExpectedType == SyntaxKind.SingleLineCommentTrivia);

            if (oTrailingTrivia != null)
            {
                // Find first possible keyword
                // if we found one, Lex everything afterwards and insert into the token list
                List<SyntaxToken> xaoNewTokens = ScanTriviaForKeywords(oTrailingTrivia);
                
                // If we have no tokens to parse
                if (xaoNewTokens.Count > 0)
                {
                    // Add the tokens
                    xoContext.List.Insert(xaoNewTokens, xoContext.List.Position);
                    return true;
                }
            }

            // Try leading trivia
            SyntaxTrivia oLeadingTrivia = oNextToken.LeadingTrivia.Find((oTrivia) =>
                oTrivia.ExpectedType == SyntaxKind.MultiLineCommentTrivia ||
                oTrivia.ExpectedType == SyntaxKind.SingleLineCommentTrivia);

            // Use the leading trivia of the next node
            if (oLeadingTrivia != null)
            {
                // Find first possible keyword
                // if we found one, Lex everything afterwards and insert into the token list
                List<SyntaxToken> xaoNewTokens = ScanTriviaForKeywords(oLeadingTrivia);

                // If we have no tokens to parse
                if (xaoNewTokens.Count > 0)
                {
                    // Add the tokens
                    xoContext.List.Insert(xaoNewTokens, xoContext.List.Position);
                    return true;
                }
            }

            // Scan Trailing of last

            return false;
        }

        /// <summary>
        /// This is very unsafe as everything is converted to Upper
        /// before the conversion (ie. it matches against any case)
        /// </summary>
        /// <param name="xoTrivia"></param>
        /// <param name="xeExpectedKind"></param>
        /// <returns></returns>
        public static List<SyntaxToken> ScanTriviaForKeywords(
            SyntaxTrivia xoTrivia, 
            Predicate<SyntaxKind> xoExpectedKind = null)
        {
            String[] asTriviaTextTokens = xoTrivia.RawSQLText.Split();
            List<SyntaxToken> xaoReturnList = new List<SyntaxToken>();

            for (int iIndex = 0; iIndex < asTriviaTextTokens.Length; iIndex++)
            {
                String sLoopingVar = asTriviaTextTokens[iIndex];
                SyntaxKind ePossibleKind = SyntaxKindUtilities.GetKindFromString(sLoopingVar);

                // If we have a positive match
                if (ePossibleKind != SyntaxKind.UnknownNode)
                {
                    if (xoExpectedKind == null || // If there is no expected fn
                        (xoExpectedKind != null && xoExpectedKind(ePossibleKind))) // Or there is one and this matches
                    {
                        int iCleanIndex = iIndex == 0 ? 0 : iIndex - 1;
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

            return xaoReturnList;
        }



    }
}
