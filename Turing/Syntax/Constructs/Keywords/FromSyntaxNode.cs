using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Parser;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Symbols;

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

                // Allow Bracket/Subqueries
                { SyntaxKind.OpenParenthesisToken },
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
                // If there is nothing trailing this
                if (Children.Count == 0)
                {
                    // Database
                    if (xoList.PeekToken().ExpectedType == SyntaxKind.DotDotToken)
                    {
                        return new DatabaseSymbol(xoToken.RawSQLText);
                    }
                    else
                    {
                        // Standalone table
                        return GenerateTableNode(xoToken, xoList);
                    }
                }
                // Now we need to determine what we are dealing with
                else if (Children[Children.Count - 1].ExpectedType == SyntaxKind.DotToken)
                {
                    return HandlePrevDotToken(xoToken, xoList);
                }
                // Leading ..
                else if (Children[Children.Count - 1].ExpectedType == SyntaxKind.DotDotToken)
                {
                    // Most likely a table (Database..Table)
                    return GenerateTableNode(xoToken, xoList);
                }
                // We have an identifier or keyword trailing
                else
                {
                    // Unknown
                    Console.WriteLine();
                    // A join keyword
                    return base.ConvertTokenIntoNode(xoToken, xoList);
                }
            }
            else
            {
                // A join keyword
                return base.ConvertTokenIntoNode(xoToken, xoList);
            }
        }

        /// <summary>
        /// Generator function for Table Nodes since we know a table
        /// can have an alias
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private SyntaxNode GenerateTableNode (SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // Create a table symbol
            TableSymbol oTable = new TableSymbol(xoToken.RawSQLText);

            // Assign the alias
            oTable.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);
            
            // Return the newly created table node
            return oTable;
        }

        /// <summary>
        /// Handles getting a single leading . token
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private SyntaxNode HandlePrevDotToken(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // Schema
            if (xoList.PeekToken().ExpectedType == SyntaxKind.DotToken)
            {
                // SCHEMA
                return new SchemaSymbol(xoToken.RawSQLText);
            }
            else
            {
                // If we have a database before the .
                if (Children[Children.Count - 2].GetType() == typeof(DatabaseSymbol))
                {
                    // They probably forgot to add a second .
                    // Change it for them
                    Children[Children.Count - 1].ExpectedType = SyntaxKind.DotDotToken;
                    Children[Children.Count - 1].RawSQLText = "..";

                    // Start this function again with the modifications in place
                    return this.ConvertTokenIntoNode(xoToken, xoList);
                }
                // Nothing before the .
                else
                {
                    // Initialise and store the symbol
                    SyntaxNode oReturnNode = new TableSymbol(xoToken.RawSQLText);

                    // Delete the last token and add a reason code
                    Children.Remove(Children[Children.Count - 1]);

                    // reason
                    oReturnNode.Comments.Add(
                        new Diagnostics.StatusItem(
                            String.Format(
                                ErrorMessageLibrary.REMOVED_NODE, // Removed Node error message
                                ReasonMessageLibrary.INCORRECT_POSITION, // REASON
                                xoToken.RawSQLText) 
                            ));

                    // Table - just done wrong..
                    return oReturnNode;
                }
            }
         
        }
    }
}
