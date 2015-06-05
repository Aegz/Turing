using System;
using Turing.Diagnostics;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Exceptions;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Strategies;

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
        public static SyntaxNode ContextSensitiveConvertTokenToNode(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eNextTokenKind = xoList.PeekToken().ExpectedType;

            // Is any identifier or Expression (function/literal)
            if (SyntaxKindFacts.IsIdentifierOrExpression(eNextTokenKind))
            {
                return FactoryCreateColumnOrExpression(xoCurrentNode, xoList);
            }

            int iMaxChildCount = -1;

            switch (eNextTokenKind)
            {
                case SyntaxKind.SelectKeyword:
                    break;
                case SyntaxKind.FromKeyword:
                case SyntaxKind.WhereKeyword:
                case SyntaxKind.OnKeyword:
                    iMaxChildCount = 1;
                    break;

                case SyntaxKind.NotKeyword:
                    return FactoryCreateNot(xoList);

                #region Operators
                case SyntaxKind.IsKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.LikeKeyword:
                case SyntaxKind.EqualsToken:
                case SyntaxKind.GreaterThanOrEqualToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.LessThanOrEqualToToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.DiamondToken:
                case SyntaxKind.AndKeyword:
                case SyntaxKind.OrKeyword:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.SlashToken:
                    iMaxChildCount = 2;
                    break;

                case SyntaxKind.StarToken:
                    // No items in the list to consume (Solitary *)
                    if (xoCurrentNode.Count == 0)
                    {
                        // 
                        return FactoryCreateColumn(xoList);
                    }
                    else
                    {
                        iMaxChildCount = 2;
                        break;
                    }

                #endregion

                #region JOIN
                case SyntaxKind.JoinKeyword:
                    // All Join nodes on their own become Inner joins
                    SyntaxNode oJoinNode = new SyntaxNode(
                        xoList.PeekToken(), 
                        NodeStrategyFactory.FactoryCreateStrategy(xoList.PopToken().ExpectedType), 
                        2);
                    oJoinNode.ExpectedType = SyntaxKind.InnerJoinKeyword;
                    return oJoinNode;
                case SyntaxKind.InnerJoinKeyword:
                case SyntaxKind.OuterKeyword:
                case SyntaxKind.LeftJoinKeyword:
                case SyntaxKind.RightJoinKeyword:
                case SyntaxKind.CrossJoinKeyword:
                    return FactoryCreateCompoundJoin(xoList);
                #endregion

                //case SyntaxKind.CaseKeyword:
                case SyntaxKind.WhenKeyword:
                case SyntaxKind.ThenKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.BarBarToken:
                    break;

                default:
                    // Default to the original token since it doesn't need to be converted
                    // any more
                    return new SyntaxLeaf(xoList.PopToken());
            }

            return new SyntaxNode(
                       xoList.PeekToken(),
                       NodeStrategyFactory.FactoryCreateStrategy(xoList.PopToken().ExpectedType),
                       iMaxChildCount); // Can have multiple children

        }

        #region Core Factory Methods

        /// <summary>
        /// A factory method for interpreting what kind of Open Parenthesis construct
        /// we have here. 
        /// 
        /// This could be:
        /// 1. SubQuery                 (SELECT * FROM X) Y
        /// 2. An Expression grouping   WHERE (X==Y) AND ((X > Z) OR (X > A))
        /// 3. SymbolList               GROUP BY (SVC_IDNTY, MKT_PROD_CD)
        /// 4. Solo Column
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private static SyntaxNode FactoryInterpretOpenParenthesisToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Given a (, Try and guess what type of () this is
            SyntaxNode oReturn;
            SyntaxToken xoCurrentToken = xoList.PopToken();

            // 1. ( SubQuery ) - (SELECT * FROM ..) svc
            // 4. SubQuery in Expression - Where X IN (SELECT Svc_IDNTY FROM svc)
           
            // First Keyword in is SELECT
            if (xoList.PeekToken().ExpectedType == SyntaxKind.SelectKeyword)
            {
                // Create SubQuery Symbol
                oReturn = FactoryCreateTable(xoList);
            }
            // 2. ( Symbol List ) - (1, 2, 3) or (svc.MKT_PROD_CD, svc.SVC_STAT_CD) 
            // 3. ( Expressions ) - WHERE (X =Y) or (X <> Y AND X = 0)
            else
            {
                Boolean bCommaFound = false; // If you find any commas before the closing bracket
                Boolean bAllIdentifiersOrDots = true;

                for (int iIndex = 0; iIndex < xoList.Count; iIndex++)
                {
                    SyntaxToken oLoopingNode = xoList.PeekToken(iIndex);
                    // Scan until we find something that helps us determine what this parenthesis is
                    // Or a close parenthesis
                    if (oLoopingNode.ExpectedType == SyntaxKind.CloseParenthesisToken ||
                        bCommaFound
                        )
                    {
                        break;
                    }
                    // Found Comma -> SymbolList
                    else if (oLoopingNode.ExpectedType == SyntaxKind.CommaToken)
                    {
                        bCommaFound = true;
                        bAllIdentifiersOrDots = false;
                    }
                    // Found an operaotr -> Expression
                    else if (
                        SyntaxKindFacts.IsAdjunctConditionalOperator(oLoopingNode.ExpectedType) || // And or
                        SyntaxKindFacts.IsConditionalOperator(oLoopingNode.ExpectedType) ||        // <> >= =
                        SyntaxKindFacts.IsArithmaticOperator(oLoopingNode.ExpectedType))           // +
                    {
                        bAllIdentifiersOrDots = false;
                    }
                    // Found something to say this is not a single column
                    else if (bAllIdentifiersOrDots && 
                        (oLoopingNode.ExpectedType != SyntaxKind.IdentifierToken ||
                        oLoopingNode.ExpectedType != SyntaxKind.DotDotToken ||
                        oLoopingNode.ExpectedType != SyntaxKind.DotToken )
                        )
                    {
                        bAllIdentifiersOrDots = false;
                    }
                }

                // Symbol List
                if (bCommaFound)
                {
                    //
                    xoCurrentToken.ExpectedType = SyntaxKind.ColumnListNode;
                    oReturn = new SymbolList(xoCurrentToken);
                }
                // Single column
                else if (bAllIdentifiersOrDots)
                {
                    // This must have an openning and closing parenthesis
                    oReturn = FactoryCreateColumn(xoList);

                    // Ignore a trailing Close parenthesis
                    if (xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
                    {
                        xoList.PopToken();
                    }
                }
                // Default to expression
                else 
                {
                    // Probably an expression type
                    return new SyntaxNode(xoCurrentToken, NodeStrategyFactory.UNARY_EXPRESSION_STRATEGY); // UnaryExpression;
                }
            }

            return oReturn;
        }

        /// <summary>
        /// Generates a simple Table Symbol (used only in FROM)
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode FactoryCreateTable(SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();

            if (xoCurrentToken.ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Subquery start
                SyntaxToken oNextToken = xoList.PopToken();
                oNextToken.ExpectedType = SyntaxKind.IdentifierSubQuerySymbol; // give it a better type
                // Create a table symbol
                Symbol oSubquery = new Symbol(oNextToken);
                return oSubquery;
            }

            SyntaxNode oDatabase;
            SyntaxNode oSchema;
            // A Symbol consists of multiple parts
            SyntaxNode oTable;

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

            oDatabase = new Symbol(new SyntaxToken(SyntaxKind.IdentifierDatabaseSymbol, 
                iTableLocation != 0 ? xoCurrentToken.RawSQLText : String.Empty));

            // Generate the Schema Node
            oSchema = new Symbol(new SyntaxToken(SyntaxKind.IdentifierSchemaSymbol, 
                    iSchemaLocation != -1 ? xoList.PeekToken(iSchemaLocation).RawSQLText : String.Empty));

            SyntaxToken oTableToken = xoList.PeekToken(iTableLocation);
            oTable = new Symbol(new SyntaxToken(SyntaxKind.IdentifierTableSymbol, oTableToken.RawSQLText));

            // create the decorator obj
            oSchema.Add(oTable);
            oDatabase.Add(oSchema);

            // Pop the tokens
            xoList.PopTokens(Math.Max(iSchemaLocation, iTableLocation) + 1);

            // Assign the alias
            ((Symbol)oTable).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

            return oDatabase;
        }

        public static SyntaxNode FactoryCreateCompoundJoin (SyntaxTokenList xoList)
        {
            // Create the Join Node
            //JoinSyntaxNode oTemp = new JoinSyntaxNode(xoList.PopToken());
            SyntaxNode oTemp = new SyntaxNode(xoList.PeekToken(), NodeStrategyFactory.FactoryCreateStrategy(xoList.PopToken().ExpectedType)); 

            // If the next node is actually an OUTER keyword
            if (xoList.PeekToken().ExpectedType == SyntaxKind.OuterKeyword)
            {
                // Construct a proper Join keyword with the type declared
                oTemp.RawSQLText += " " + xoList.PopToken().RawSQLText; // add the text (OUTER)
            }
            // If the next node is actually a Join
            if (xoList.PeekToken().ExpectedType == SyntaxKind.JoinKeyword)
            {
                // Construct a proper Join keyword with the type declared
                oTemp.RawSQLText += " " + xoList.PopToken().RawSQLText; // add the text (JOIN)
            }
            else
            {
                // Add an error
                oTemp.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8000, xoList.PopToken().RawSQLText, "JOIN"));

            }
            // Return the Join node
            return oTemp;
        }

        public static SyntaxNode FactoryCreateNot (SyntaxTokenList xoList)
        {
            int iMaxChildCount = 1;
            SyntaxToken oReturn = xoList.PopToken();
            if (xoList.PeekToken().ExpectedType == SyntaxKind.InKeyword)
            {
                oReturn.ExpectedType = SyntaxKind.NotInKeyword;
                oReturn.RawSQLText += " " + xoList.PopToken().RawSQLText;
                iMaxChildCount = 2;
            }
            else if (xoList.PeekToken().ExpectedType == SyntaxKind.LikeKeyword)
            {
                oReturn.ExpectedType = SyntaxKind.NotLikeKeyword;
                oReturn.RawSQLText += " " + xoList.PopToken().RawSQLText;
                iMaxChildCount = 2;
            }

            return new SyntaxNode(oReturn, NodeStrategyFactory.FactoryCreateStrategy(oReturn.ExpectedType), iMaxChildCount);
        }

        public static SyntaxNode FactoryCreateColumnOrExpression (SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eNextTokenKind = xoList.PeekToken().ExpectedType;
            SyntaxNode oReturnNode;

            // Is any identifier or Expression (function/literal)
            if (SyntaxKindFacts.IsFunction(eNextTokenKind))
            {
                // COUNT (*)
                oReturnNode = FactoryCreateColumnFunction(xoList);
            }
            else if (eNextTokenKind == SyntaxKind.OpenParenthesisToken)
            {
                // Purely an identifier
                oReturnNode = FactoryInterpretOpenParenthesisToken(xoCurrentNode, xoList);
            }
            else if (
                SyntaxKindFacts.IsIdentifier(eNextTokenKind) ||
                SyntaxKindFacts.IsLiteral(eNextTokenKind))
            {
                // Purely an identifier
                oReturnNode = FactoryCreateColumn(xoList);
            }
            else if (eNextTokenKind == SyntaxKind.CaseKeyword)
            {
                if (xoCurrentNode.ExpectedType == SyntaxKind.ColumnListNode)
                {
                    // Purely an identifier
                    return new Symbol(
                               xoList.PeekToken(),
                               NodeStrategyFactory.FactoryCreateStrategy(xoList.PopToken().ExpectedType), -1);
                }
                else
                {
                    // Purely an identifier
                    return new SyntaxNode(
                               xoList.PeekToken(),
                               NodeStrategyFactory.FactoryCreateStrategy(xoList.PopToken().ExpectedType));
                }

            }
            else
            {
                oReturnNode = new SyntaxLeaf(xoList.PopToken());
            }
            // CASE

            // If we have a trailing || (and we arent already using a bar bar
            if (xoCurrentNode.ExpectedType != SyntaxKind.BarBarToken &&
                xoList.PeekToken().ExpectedType == SyntaxKind.BarBarToken)
            {
                SyntaxNode oBarNode = new SyntaxNode(xoList.PopToken());
                oBarNode.Add(oReturnNode);

                return oBarNode;         
            }
            // Valid case, no fixing necessary
            else
            {
                return oReturnNode;
            }
        }

        #region Column Identifier/Function 
        /// <summary>
        /// Generates a Column Symbol (Used everywhere else)
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode FactoryCreateColumn(SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();

            // A Symbol consists of multiple parts
            Symbol oTable;
            Symbol oColumn;

            // If this is a literal Column
            if (SyntaxKindFacts.IsLiteral(xoCurrentToken.ExpectedType))
            {
                oColumn = new Symbol(xoList.PopToken(), NodeStrategyFactory.NULL_STRATEGY);

                // Assign the alias
                oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return oColumn;
            }

            // Trailing item is . (table.column)
            if (xoList.PeekToken(1).ExpectedType == SyntaxKind.DotToken)
            {
                oTable = new Symbol(xoCurrentToken, NodeStrategyFactory.NULL_STRATEGY);
                oColumn = new Symbol(xoList.PeekToken(2)); // Grab the Column
                oTable.Add(oColumn);
                xoList.PopTokens(3); // Skip over the next 2
            }
            // Standalone Column
            else
            {
                oTable = new Symbol(new SyntaxToken(SyntaxKind.IdentifierTableSymbol, String.Empty));
                oColumn = new Symbol(xoCurrentToken, NodeStrategyFactory.NULL_STRATEGY); // Grab the Column
                oTable.Add(oColumn);
                xoList.PopToken(); // Skip over the next 1
            }


            // Assign the alias
            oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

            oColumn.ExpectedType = SyntaxKind.IdentifierColumnSymbol;
            oTable.ExpectedType = SyntaxKind.IdentifierTableSymbol;

            // Return the top level node
            return oTable;
        }

        public static SyntaxNode FactoryCreateColumnFunction(SyntaxTokenList xoList)
        {
            // generate the item
            Symbol oColumnExp = new Symbol(xoList.PopToken());

            // Consume Ahead
            oColumnExp.TryConsumeList(xoList);

            // Assign the alias
            oColumnExp.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

            // Return the column
            return oColumnExp;
        }
        #endregion

        #endregion

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
