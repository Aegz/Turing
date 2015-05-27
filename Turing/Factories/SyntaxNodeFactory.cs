using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Expressions;
using Turing.Syntax.Constructs.Keywords;
using Turing.Syntax.Constructs.Symbols.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Factories
{
    class SyntaxNodeFactory
    {
        /// <summary>
        /// Context sensitive conversion which will scan ahead to determine the best candidate
        /// for this node
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode ContextSensitiveConvertTokenToNode(SyntaxTokenList xoList)
        {
            // Always pop on entering this fn
            SyntaxToken xoCurrentToken = xoList.PopToken();

            //return new SyntaxNode(xoCurrentToken, NodeStrategyFactory.FactoryCreateStrategy(xoCurrentToken.ExpectedType));
            switch (xoCurrentToken.ExpectedType)
            {
                case SyntaxKind.SelectKeyword:
                case SyntaxKind.FromKeyword:
                case SyntaxKind.WhereKeyword:
                case SyntaxKind.OnKeyword:
                    return new SyntaxNode(xoCurrentToken, NodeStrategyFactory.FactoryCreateStrategy(xoCurrentToken.ExpectedType));

                #region JOIN
                case SyntaxKind.JoinKeyword:
                    // All Join nodes on their own become Inner joins
                    SyntaxNode oJoinNode = new JoinSyntaxNode(xoCurrentToken);
                    oJoinNode.ExpectedType = SyntaxKind.InnerJoinKeyword;
                    return oJoinNode;
                case SyntaxKind.InnerJoinKeyword:
                case SyntaxKind.OuterKeyword:
                case SyntaxKind.LeftJoinKeyword:
                case SyntaxKind.RightJoinKeyword:
                case SyntaxKind.CrossJoinKeyword:
                    // Create the Join Node
                    JoinSyntaxNode oTemp = new JoinSyntaxNode(xoCurrentToken);

                    // If the next node is actually an OUTER keyword
                    if (xoList.PeekToken().ExpectedType == SyntaxKind.OuterKeyword)
                    {
                        // Construct a proper Join keyword with the type declared
                        oTemp.RawSQLText += " " + xoList.PeekToken().RawSQLText; // add the text (OUTER)
                        xoList.PopToken(); // Pull it off the list
                    }

                    // If the next node is actually a Join
                    if (xoList.PeekToken().ExpectedType == SyntaxKind.JoinKeyword)
                    {
                        // Construct a proper Join keyword with the type declared
                        oTemp.ExpectedType = xoCurrentToken.ExpectedType; // Set the type
                        oTemp.RawSQLText += " " + xoList.PeekToken().RawSQLText; // add the text (JOIN)
                        xoList.PopToken(); // Pull it off the list
                    }
                    else
                    {
                        // Add an error
                        oTemp.InsertStatusMessage(
                            String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xoList.PeekToken().RawSQLText, "JOIN"));
                    }
                    // Return the Join node
                    return oTemp;
                #endregion


                #region Conditional Expressions
                case SyntaxKind.EqualsToken:
                case SyntaxKind.GreaterThanOrEqualToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.LessThanOrEqualToToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.DiamondToken:
                case SyntaxKind.AndKeyword:
                case SyntaxKind.OrKeyword:
                    // Return a boolean expression (which will consume the previous node and the next one)
                    return new BinaryExpression(xoCurrentToken);

                #endregion

                // The type of the expression will be determined by the Token's
                // Type and we will use this to compare compatibility
                case SyntaxKind.LiteralToken:
                case SyntaxKind.BooleanToken: // true and false and NOT
                case SyntaxKind.NumericToken:
                    return new SyntaxLeaf(xoCurrentToken);

                case SyntaxKind.OpenParenthesisToken:
                    // Given a (, Try and guess what type of () this is
                    return FactoryInterpretOpenParenthesisToken(xoCurrentToken, xoList);
                case SyntaxKind.NotKeyword:
                    return new UnaryExpression(xoCurrentToken);

                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return new SyntaxLeaf(xoCurrentToken);
            }
        }

        #region Factory Methods

        /// <summary>
        /// A factory method for interpreting what kind of Open Parenthesis construct
        /// we have here. 
        /// 
        /// This could be:
        /// 1. SubQuery                 (SELECT * FROM X) Y
        /// 2. An Expression grouping   WHERE (X==Y) AND ((X > Z) OR (X > A))
        /// 3. SymbolList               GROUP BY (SVC_IDNTY, MKT_PROD_CD)
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private static SyntaxNode FactoryInterpretOpenParenthesisToken(SyntaxToken xoCurrentToken, SyntaxTokenList xoList)
        {
            // Given a (, Try and guess what type of () this is
            SyntaxNode oReturn;

            // 1. ( SubQuery ) - (SELECT * FROM ..) svc
            // 4. SubQuery in Expression - Where X IN (SELECT Svc_IDNTY FROM svc)
            // ?? TODO: Fix this one because it should be SymbolList
            // First Keyword in is SELECT
            if (xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword)
            {
                // Create SubQuery Symbol
                oReturn = SymbolFactory.GenerateTableSymbol(xoList);
            }
            // 2. ( Symbol List ) - (1, 2, 3) or (svc.MKT_PROD_CD, svc.SVC_STAT_CD) 
            // 3. ( Expressions ) - WHERE (X =Y) or (X <> Y AND X = 0)
            else
            {
                Boolean bCommaFound = false; // If you find any commas before the closing bracket
                Boolean bOperatorFound = false; // If you find any operators before the closing bracket
                for (int iIndex = 0; iIndex < xoList.Count; iIndex++)
                {
                    SyntaxToken oLoopingNode = xoList.PeekToken(iIndex);
                    if (oLoopingNode.ExpectedType == SyntaxKind.CloseParenthesisToken ||
                        bCommaFound ||
                        bOperatorFound)
                    {
                        break;
                    }
                    else if (oLoopingNode.ExpectedType == SyntaxKind.CommaToken)
                    {
                        bCommaFound = true;
                    }
                    else if (SyntaxNode.IsOperator(oLoopingNode.ExpectedType))
                    {
                        bOperatorFound = true;
                    }
                }

                // EXPRESSION
                if (bOperatorFound)
                {
                    // Most likely an expression list
                    oReturn = new UnaryExpression(xoCurrentToken);
                }
                // Symbol List
                else if (bCommaFound)
                {
                    //
                    oReturn = new SymbolList(xoCurrentToken);
                }
                // Single expression?
                else if (xoList.PeekToken().ExpectedType == SyntaxKind.IdentifierToken)
                {
                    // ?? TODO: FIX THis, very risky

                    // It could be just one identifier on its own
                    oReturn = SymbolFactory.GenerateColumnSymbol(xoList);
                }
                else // Both empty
                {
                    // Probably an expression type
                    return new UnaryExpression(xoCurrentToken);
                }
            }


            // Have the child consume?
            //oReturn.TryConsumeList(xoList);
            // Skip it
            //xoList.PopToken();
            return oReturn;
        }


        public static SyntaxNode FactoryCreateExceptionNodeWithExpectingError(String xsExpected, String xsRawSQL)
        {
            // Error
            SyntaxNode oError = new ExceptionSyntaxNode();
            oError.InsertStatusMessage(
                String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xsExpected, xsRawSQL));
            return oError;
        }


        #endregion


        #region Common Functions

        /// <summary>
        /// Scans ahead for a possibly Identifier/Alias and returns the value 
        /// if it finds one. if it does find one, it will move the Window accordingly
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static String ScanAheadForAlias(SyntaxTokenList xoList)
        {
            String sAlias = String.Empty;
            // SCAN AHEAD - To grab an alias if there is one
            if (xoList.HasTokensLeftToProcess())
            {
                SyntaxToken oNextNode = xoList.PeekToken();
                // Explicit
                if (oNextNode.ExpectedType == SyntaxKind.AsKeyword)
                {
                    // And the next node after that is an identifier
                    if (xoList.PeekToken(1).ExpectedType == SyntaxKind.IdentifierToken)
                    {
                        // Alias found
                        sAlias = xoList.PeekToken(1).RawSQLText;
                        xoList.PopTokens(2);
                    }
                }
                // Implicit
                else if (oNextNode.ExpectedType == SyntaxKind.IdentifierToken)
                {
                    // Alias found
                    sAlias = oNextNode.RawSQLText;
                    xoList.PopToken();
                }
            }

            // Return the newly created table node
            return sAlias;
        }

       #endregion

    }
}
