using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.SingleChild;

namespace Turing.Factories
{
    class SymbolFactory
    {
        /// <summary>
        /// Generates a simple Table Symbol
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Symbol GenerateTableSymbol(SyntaxToken xoCurrentToken, SyntaxTokenList xoList)
        {
            Symbol oDatabase;
            Symbol oSchema;
            // A Symbol consists of multiple parts
            Symbol oTable;

            int iSchemaLocation = 0;
            int iTableLocation = 0;

            // Trailing item is a .. (Database)
            if (xoList.PeekToken().ExpectedType == SyntaxKind.DotDotToken)
            {
                iSchemaLocation = -1;
                iTableLocation = 1;
            }
            else if (xoList.PeekToken().ExpectedType == SyntaxKind.DotToken)
            {
                iSchemaLocation = 1;
                iTableLocation = 3;
            }
            // Standalone Table
            else
            {
                iSchemaLocation = -1;
                iTableLocation = 0;
            }

            oDatabase =
                iTableLocation != 0 ? // Theres no database decl or schema decl
                new DatabaseSymbol(xoCurrentToken) :
                new DatabaseSymbol(SyntaxToken.NULL_TOKEN);

            // Generate the Schema Node
            oSchema = iSchemaLocation != -1 ?
                // Type Check
                xoList.PeekToken(iSchemaLocation).ExpectedType == SyntaxKind.IdentifierToken ?
                    new SchemaSymbol(xoList.PeekToken(iSchemaLocation)) :
                    GenerateMissingButExpectedSymbol("SchemaIdn", xoList.PeekToken(iSchemaLocation).RawSQLText)
                :
                new SchemaSymbol(SyntaxToken.NULL_TOKEN);

            oTable = iTableLocation != -1 ?
                // Type check
                xoList.PeekToken(iTableLocation).ExpectedType == SyntaxKind.IdentifierToken ?
                    new TableSymbol(xoList.PeekToken(iTableLocation)) :
                    GenerateMissingButExpectedSymbol("TableIdn", xoList.PeekToken(iTableLocation).RawSQLText) :
                new TableSymbol(SyntaxToken.NULL_TOKEN);

            // create the decorator obj
            oSchema.AddChild(oTable);
            oDatabase.AddChild(oSchema);
            // Pop the tokens
            xoList.PopTokens(Math.Max(iSchemaLocation, iTableLocation) + 1);

            // Assign the alias
            oTable.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

            return oDatabase;
        }
        
        /// <summary>
        /// Generate a TableSymbol which encompasses a Subquery
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode GenerateTableSymbol(SyntaxNode xoCurrentNode, SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // If we get a select statement next
            if (xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword)
            {
                // Create a table symbol
                TableSymbol oTable = new TableSymbol(xoToken);

                // Add the parenthesis (
                oTable.AddChild(new SyntaxTokenWrapper(xoToken));

                // Build a Select node
                SyntaxNode oSelect = xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword ?
                    SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoList.PopToken(), xoList) :
                    SymbolFactory.GenerateMissingButExpectedSymbol("SELECT", xoList.PopToken().RawSQLText); // Return an error node if we need to

                // Add it to the Subquery Symbol
                oTable.AddChild(oSelect);

                // Try and build the Select statement
                oSelect.TryConsumeList(xoList);

                // Add the parenthesis )
                if (xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
                {
                    xoCurrentNode.AddChild(new SyntaxTokenWrapper(xoList.PopToken()));
                }

                // Assign the alias
                oTable.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return oTable;
            }
            else
            {
                // Error Node, expecting SELECT
                SyntaxNode oException = new ExceptionSyntaxNode();
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

        /// <summary>
        /// Generates a Column Symbol
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Symbol GenerateColumnSymbol(SyntaxToken xoCurrentToken, SyntaxTokenList xoList)
        {
            // A Symbol consists of multiple parts
            Symbol oTable;
            Symbol oColumn;

            // Trailing item is . (table.column)
            if (xoList.PeekToken().ExpectedType == SyntaxKind.DotToken)
            {
                oTable = new TableSymbol(xoCurrentToken);
                oColumn = new ColumnSymbol(xoList.PeekToken(1)); // Grab the Column
                oTable.AddChild(oColumn);
                xoList.PopTokens(2); // Skip over the next 2
            }
            // Standalone Column
            else
            {
                oTable = new TableSymbol(SyntaxToken.NULL_TOKEN);
                oColumn = new ColumnSymbol(xoCurrentToken); // Grab the Column
                oTable.AddChild(oColumn);
            }


            // Assign the alias
            oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

            // Return the top level node
            return oTable;
        }


        public static Symbol GenerateMissingButExpectedSymbol(String xsExpected, String xsRawSQL)
        {
            // Error
            Symbol oError = new NullSymbol(SyntaxToken.NULL_TOKEN);
            oError.Comments.Add(
                new StatusItem(
                    String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xsExpected, xsRawSQL)));
            return oError;
        }

    }
}
