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
        public static Symbol GenerateTableSymbol(SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();

            if (xoCurrentToken.ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Subquery start
                // Create a table symbol
                TableSymbol oSubquery = new SubquerySymbol(xoList.PopToken());
                return oSubquery;
            }

            Symbol oDatabase;
            Symbol oSchema;
            // A Symbol consists of multiple parts
            Symbol oTable;

            int iSchemaLocation = 0;
            int iTableLocation = 0;

            // Trailing item is a .. (Database)
            if (xoList.PeekToken(1).ExpectedType == SyntaxKind.DotDotToken)
            {
                iSchemaLocation = -1;
                iTableLocation = 2;
            }
            else if (xoList.PeekToken(1).ExpectedType == SyntaxKind.DotToken)
            {
                iSchemaLocation = 2;
                iTableLocation = 4;
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
                new DatabaseSymbol(new SyntaxToken(SyntaxKind.IdentifierToken, String.Empty));

            // Generate the Schema Node
            oSchema = iSchemaLocation != -1 ?
                // Type Check
                xoList.PeekToken(iSchemaLocation).ExpectedType == SyntaxKind.IdentifierToken ?
                    new SchemaSymbol(xoList.PeekToken(iSchemaLocation)) :
                    CreateExceptionNodeWithExpectingMessage(
                        "SchemaIdn", xoList.PeekToken(iSchemaLocation).RawSQLText)
                :
                new SchemaSymbol(new SyntaxToken(SyntaxKind.IdentifierToken, String.Empty));

            SyntaxToken oTableToken = xoList.PeekToken(iTableLocation);
            oTable = iTableLocation != -1 ?
                // Type check
                oTableToken.ExpectedType == SyntaxKind.IdentifierToken ?
                    new TableSymbol(oTableToken) :
                    CreateExceptionNodeWithExpectingMessage("TableIdn", oTableToken.RawSQLText) :
                new TableSymbol(new SyntaxToken(SyntaxKind.IdentifierToken, String.Empty));

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
        public static Symbol GenerateColumnSymbol(SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();

            // A Symbol consists of multiple parts
            Symbol oTable;
            Symbol oColumn;

            // Trailing item is . (table.column)
            if (xoList.PeekToken(1).ExpectedType == SyntaxKind.DotToken)
            {
                oTable = new TableSymbol(xoCurrentToken);
                oColumn = new ColumnSymbol(xoList.PeekToken(2)); // Grab the Column
                oTable.AddChild(oColumn);
                xoList.PopTokens(3); // Skip over the next 2
            }
            // Standalone Column
            else
            {
                oTable = new TableSymbol(new SyntaxToken(SyntaxKind.IdentifierToken, String.Empty));
                oColumn = new ColumnSymbol(xoCurrentToken); // Grab the Column
                oTable.AddChild(oColumn);
                xoList.PopToken(); // Skip over the next 1
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
