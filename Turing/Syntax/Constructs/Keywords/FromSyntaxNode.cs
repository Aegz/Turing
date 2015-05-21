using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Parser;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.SingleChild;

namespace Turing.Syntax.Constructs.Keywords
{
    class FromSyntaxNode : SyntaxNode
    {
        public FromSyntaxNode(String xsRawText) : base (SyntaxKind.FromKeyword, xsRawText)
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
                { SyntaxKind.OnKeyword },

                // Allow Bracket/Subqueries
                { SyntaxKind.OpenParenthesisToken },
                //{ SyntaxKind.SelectKeyword },
                { SyntaxKind.CloseParenthesisToken },

                // Grammar
                { SyntaxKind.DotDotToken },
                { SyntaxKind.DotToken },

            });
        }

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
                return GenerateSubQueryNode(xoToken, xoList);
            }
            // Any Join keyword
            else if (xoToken.ExpectedType == SyntaxKind.JoinKeyword ||
                SyntaxNodeFactory.IsJoinTypeKeyword(xoToken.ExpectedType))
            {
                // Exit early on error
                if (Children.Count == 0)
                {
                    return SymbolFactory.GenerateMissingButExpectedSymbol("NONE", "TableDecl");
                }

                SyntaxNode oPrevNode = Children[Children.Count - 1];
                // If we have a table before this or another JOIN structure
                if (oPrevNode.ExpectedType == SyntaxKind.IdentifierToken ||
                    oPrevNode.ExpectedType == SyntaxKind.JoinKeyword ||
                    SyntaxNodeFactory.IsJoinTypeKeyword(oPrevNode.ExpectedType))
                {
                    // Create the correct join node
                    SyntaxNode oJoin = SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoToken, xoList);

                    // Consume that node as our LEFT
                    oJoin.AddChild(oPrevNode);

                    // Remove from this node's children
                    Children.RemoveAt(Children.Count - 1);

                    // Store this Join as the last node
                    Children.Add(oJoin);

                    // Ask the Join to try and consume anything it can
                    oJoin.TryConsumeList(xoList);
                }

                // Create a Subquery statement?
                return GenerateSubQueryNode(xoToken, xoList);
            }
            else
            {
                // Everything else
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }


        /// <summary>
        /// Generate a TableSymbol which encompasses a Subquery
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private SyntaxNode GenerateSubQueryNode (SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // If we get a select statement next
            if (xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword)
            {
                // Create a table symbol
                TableSymbol oTable = new TableSymbol(xoToken.RawSQLText);

                // Add the parenthesis (
                oTable.AddChild(xoToken);

                // Build a Select node
                SyntaxNode oSelect = xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword ? 
                    SyntaxNodeFactory.NonContextSensitiveConvertTokenToNode(xoList.PopToken()) :
                    SymbolFactory.GenerateMissingButExpectedSymbol("SELECT", xoList.PopToken().RawSQLText); // Return an error node if we need to

                // Add it to the Subquery Symbol
                oTable.AddChild(oSelect);

                // Try and build the Select statement
                oSelect.TryConsumeList(xoList);

                // Add the parenthesis )
                if (xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
                {
                    AddChild(xoList.PopToken());
                }

                // Assign the alias
                oTable.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return oSelect;
            }
            else
            {
                // Error Node, expecting SELECT
                SyntaxNode oException = new ExceptionSyntaxToken();
                oException.Comments.Add(
                    new StatusItem(
                        String.Format(
                            ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE,
                            "SELECT",
                            xoToken.RawSQLText
                            )));
                return oException;
            }
        }     
    }
}
