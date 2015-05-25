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
            if (xoCurrentToken.ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Subquery start
                // Create a table symbol
                TableSymbol oSubquery = new SubquerySymbol(SyntaxToken.NULL_TOKEN);

                // Add the parenthesis (
                //oSubquery.AddChild(new SyntaxLeaf(xoCurrentToken));

                // Build a Select node
                SyntaxNode oSelect = xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword ?
                    SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoList.PopToken(), xoList) :
                    SyntaxNodeFactory.CreateExceptionNodeWithExpectingMessage("SELECT", xoList.PopToken().RawSQLText); // Return an error node if we need to

                // Add it to the Subquery Symbol
                oSubquery.AddChild(oSelect);

                // Try and build the Select statement
                oSelect.TryConsumeList(xoList);


                // Add the parenthesis )
                if (xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
                {
                    xoList.PopToken();
                    //oSubquery.AddChild(new SyntaxLeaf(xoList.PopToken()));
                }

                // Assign the alias
                oSubquery.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return oSubquery;
            }

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
                    CreateExceptionNodeWithExpectingMessage(
                        "SchemaIdn", xoList.PeekToken(iSchemaLocation).RawSQLText)
                :
                new SchemaSymbol(SyntaxToken.NULL_TOKEN);

            SyntaxToken oTableToken = xoList.PeekToken(iTableLocation);
            oTable = iTableLocation != -1 ?
                // Type check
                oTableToken.ExpectedType == SyntaxKind.IdentifierToken ?
                    new TableSymbol(oTableToken) :
                    CreateExceptionNodeWithExpectingMessage("TableIdn", oTableToken.RawSQLText) :
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



        public static Symbol CreateExceptionNodeWithExpectingMessage(String xsExpected, String xsRawSQL)
        {
            // Error
            Symbol oError = new NullSymbol(SyntaxToken.NULL_TOKEN);
            oError.InsertStatusMessage(
                String.Format(ErrorMessageLibrary.EXPECTING_TOKEN_FOUND_ELSE, xsExpected, xsRawSQL));
            return oError;
        }
    }
}
