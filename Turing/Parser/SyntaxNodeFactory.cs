using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
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
                    return new SelectSyntaxNode(xoToken.RawSQLText);
                case SyntaxKind.FromKeyword:
                    return new FromSyntaxNode(xoToken.RawSQLText);
                
                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return xoToken;
            }
        }

        /// <summary>
        /// Context sensitive conversion which will scan ahead to determine the best candidate
        /// for this node
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode ContextSensitiveConvertTokenToNode(SyntaxNode xoCurrentToken, SyntaxTokenList xoList)
        {
            switch (xoCurrentToken.ExpectedType)
            {
                case SyntaxKind.SelectKeyword:
                    return new SelectSyntaxNode(xoCurrentToken.RawSQLText);
                case SyntaxKind.FromKeyword:
                    return new FromSyntaxNode(xoCurrentToken.RawSQLText);
                case SyntaxKind.JoinKeyword:
                    return new JoinKeyword(xoCurrentToken.RawSQLText);
                case SyntaxKind.InnerJoinKeyword:
                case SyntaxKind.OuterJoinKeyword:
                case SyntaxKind.LeftJoinKeyword:
                case SyntaxKind.RightJoinKeyword:
                case SyntaxKind.CrossJoinKeyword:
                    // Create the Join Node
                    SyntaxNode oTemp = new JoinKeyword(xoCurrentToken.RawSQLText);
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
                    return new OnKeyword(xoCurrentToken.RawSQLText);
                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return xoCurrentToken;
            }
        }

        public static Boolean IsJoinTypeKeyword(SyntaxKind xeType)
        {
            return
                xeType == SyntaxKind.InnerJoinKeyword ||
                xeType == SyntaxKind.LeftJoinKeyword ||
                xeType == SyntaxKind.RightJoinKeyword ||
                xeType == SyntaxKind.CrossJoinKeyword ||
                xeType == SyntaxKind.OuterJoinKeyword;
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
