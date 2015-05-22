using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Expressions;
using Turing.Syntax.Constructs.Keywords;

namespace Turing.Parser
{
    class SyntaxNodeFactory
    {
        /// <summary>
        /// Generic Non context sensitive conversion
        /// </summary>
        /// <param name="xoToken"></param>
        /// <returns></returns>
        public static SyntaxNode NonContextSensitiveConvertTokenToNode(SyntaxToken xoToken)
        {
            switch (xoToken.ExpectedType)
            {
                case SyntaxKind.SelectKeyword:
                    return new SelectSyntaxNode(xoToken);
                case SyntaxKind.FromKeyword:
                    return new FromSyntaxNode(xoToken);
                
                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return new SyntaxTokenWrapper(xoToken);
            }
        }

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
                case SyntaxKind.JoinKeyword:
                    return new JoinSyntaxNode(xoCurrentToken);
                case SyntaxKind.InnerJoinKeyword:
                case SyntaxKind.OuterJoinKeyword:
                case SyntaxKind.LeftJoinKeyword:
                case SyntaxKind.RightJoinKeyword:
                case SyntaxKind.CrossJoinKeyword:
                    // Create the Join Node
                    SyntaxNode oTemp = new JoinSyntaxNode(xoCurrentToken);
                    // If the next node is actually a Join
                    if (xoList.PeekToken().ExpectedType == SyntaxKind.JoinKeyword)
                    {
                        // Construct a proper Join keyword with the type declared
                        oTemp.ExpectedType = xoCurrentToken.ExpectedType; // Set the type
                        oTemp.RawSQLText += " " + xoList.PeekToken().RawSQLText; // add the text
                        xoList.PopToken(); // Pull it off the list
                    }
                    else
                    {
                        // Add an error
                        oTemp.Comments.Add(new StatusItem(
                            String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xoList.PeekToken().RawSQLText, "JOIN")));
                    }
                    // Return the Join node
                    return oTemp;
                case SyntaxKind.OnKeyword:
                    return new OnSyntaxNode(xoCurrentToken);

                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return new SyntaxTokenWrapper(xoCurrentToken);
            }
        }


        public static SyntaxNode ConstructExpressionNode(SyntaxToken xoCurrentToken, SyntaxTokenList xoList)
        {
            // Handle the more unique cases first
            // We start the condition with a NOT (entirely possible)
            if (xoCurrentToken.ExpectedType == SyntaxKind.NotKeyword)
            {
                // NOT (Boolean Expression)
                SyntaxNode oNot = new BooleanExpressionSyntaxNode(xoCurrentToken);
            }
            // If we have an identifier followed by an operator of some sort
            else if (SyntaxNode.IsIdentifier(xoCurrentToken.ExpectedType) || SyntaxNode.IsOperator(xoCurrentToken.ExpectedType))
            {

            }
            return null;
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

        #endregion
        
    }
}
