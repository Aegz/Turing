using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                
                // Just leave JOINS exactly as they are
                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return xoToken;
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
                // Implicit
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

        //public static SyntaxNode test(SyntaxKind xeKind, SyntaxTokenList xoList)
        //{
        //    // 1. Construct a Node based off the kind
        //    SyntaxNode oNode = ConvertTokenIntoNode(xeKind);

        //    // 2. Depth first traversal based on the node itself
        //    if (oNode.TryConsumeList(xoList))
        //    {
        //        // If it successfully consumed something
        //    }

        //    // 3. Return this object after it consumed all the tokens it could
        //    return oNode;
        //}
    }
}
