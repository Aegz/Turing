using System;
using System.Collections.Generic;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.SingleChild;

namespace Turing.Syntax.Constructs.Keywords
{
    class FromSyntaxNode : SyntaxNode
    {
        public FromSyntaxNode(SyntaxToken xoToken) : base(xoToken)
        {
            AcceptedTypes.AddRange(new List<SyntaxKind>
            {
                // Database and Table Identifiers only
                { SyntaxKind.IdentifierToken },
                { SyntaxKind.AsKeyword }, // Allow Aliasing

                // JOIN Keywords
                { SyntaxKind.JoinKeyword },
                { SyntaxKind.InnerJoinKeyword },
                { SyntaxKind.OuterJoinKeyword },
                { SyntaxKind.CrossJoinKeyword },
                { SyntaxKind.LeftJoinKeyword },
                { SyntaxKind.RightJoinKeyword },

                // JOIN additional keywords
                // This needs to be here since JOIN and ON are instantiated at different times
                //{ SyntaxKind.OnKeyword },

                // Allow Bracket/Subqueries
                { SyntaxKind.OpenParenthesisToken },
                //{ SyntaxKind.SelectKeyword },
                { SyntaxKind.CloseParenthesisToken },

                // Grammar
                { SyntaxKind.DotDotToken },
                { SyntaxKind.DotToken },

            });
        }


        /// <summary>
        /// Scans ahead and generates anything it knows how to
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public override SyntaxNode ConvertTokenIntoNode(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // If we need to perform a context sensitive conversion
            if (SyntaxNode.IsIdentifier(xoToken.ExpectedType)) // Generic Identifiers only
            {
                return SymbolFactory.GenerateTableSymbol(xoToken, xoList);
            }
            // ( - Subquery
            else if (xoToken.ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Create a Subquery statement?
                return SymbolFactory.GenerateTableSymbol(this, xoToken, xoList);
            }
            // Any Join keyword
            //else if (xoToken.ExpectedType == SyntaxKind.JoinKeyword ||
            //    SyntaxNode.IsJoinTypeKeyword(xoToken.ExpectedType))
            //{
            //    // Exit early on error
            //    if (Children.Count == 0)
            //    {
            //        return SymbolFactory.GenerateMissingButExpectedSymbol("NONE", "TableDecl");
            //    }
            //    else
            //    {
            //        // Create a Join Node
            //        SyntaxNode oJoin = SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoToken, xoList);
              
            //        // Create a Subquery statement?
            //        return oJoin; 

            //    }
            //}
            else
            {
                // Everything else
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }

  
    }
}
