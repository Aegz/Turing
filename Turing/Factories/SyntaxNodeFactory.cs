using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Expressions;
using Turing.Syntax.Constructs.Keywords;
using Turing.Syntax.Constructs.Symbols.Standalone;

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
        public static SyntaxNode ContextSensitiveConvertTokenToNode(SyntaxToken xoCurrentToken, SyntaxTokenList xoList)
        {
            switch (xoCurrentToken.ExpectedType)
            {
                case SyntaxKind.SelectKeyword:
                    return new SelectSyntaxNode(xoCurrentToken);
                case SyntaxKind.FromKeyword:
                    return new FromSyntaxNode(xoCurrentToken);
                case SyntaxKind.WhereKeyword:
                    return new WhereSyntaxNode(xoCurrentToken);

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
                        oTemp.IsOuter = true;
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

                case SyntaxKind.OnKeyword:
                    return new OnSyntaxNode(xoCurrentToken);

                case SyntaxKind.NotKeyword:
                    return new UnaryExpression(xoCurrentToken);
                case SyntaxKind.BooleanToken: // true and false and NOT
                    return new BooleanSymbol(xoCurrentToken);

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

                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return new SyntaxLeaf(xoCurrentToken);
            }
        }


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


        public static SyntaxNode CreateExceptionNodeWithExpectingMessage(String xsExpected, String xsRawSQL)
        {
            // Error
            SyntaxNode oError = new ExceptionSyntaxNode();
            oError.InsertStatusMessage(
                String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xsExpected, xsRawSQL));
            return oError;
        }
        #endregion

    }
}
