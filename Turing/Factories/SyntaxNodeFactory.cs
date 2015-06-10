using System;
using Turing.Diagnostics;
using Turing.Parser;
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
        public static SyntaxNode ContextSensitiveConvertTokenToNode(
            ParsingContext xoContext)
        {
            SyntaxKind eNextTokenKind = xoContext.List.Peek().ExpectedType;

            // Is any identifier or Expression (function/literal)
            if (SyntaxKindFacts.IsIdentifierOrExpression(eNextTokenKind))
            {
                return FactoryCreateColumnOrExpression(xoContext);
            }
            else if (SyntaxKindFacts.IsJoinKeyword(eNextTokenKind))
            {
                return FactoryCreateCompoundJoin(xoContext);
            }
            else
            {
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
                        return FactoryCreateNot(xoContext);

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
                        if (xoContext.CurrentNode.Count == 0)
                        {
                            // 
                            return FactoryCreateColumn(xoContext);
                        }
                        else
                        {
                            iMaxChildCount = 2;
                            break;
                        }

                    #endregion

                    //case SyntaxKind.CaseKeyword:
                    case SyntaxKind.WhenKeyword:
                    case SyntaxKind.ThenKeyword:
                    case SyntaxKind.ElseKeyword:
                    case SyntaxKind.BarBarToken:
                        break;

                }

                return new SyntaxNode(
                           xoContext.List.Peek(),
                           NodeStrategyFactory.FactoryCreateStrategy(xoContext.List.Pop().ExpectedType),
                           iMaxChildCount); // Can have multiple children
            }
        }

        #region Core Factory Methods

        /// <summary>
        /// Generates a simple Table Symbol 
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode FactoryCreateTable(ParsingContext xoContext)
        {
            ISyntax xoCurrentToken = xoContext.List.Peek();

            if (xoCurrentToken.ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Subquery start
                ISyntax oNextToken = xoContext.List.Pop();
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
            if (xoContext.List.Peek(1).ExpectedType == SyntaxKind.DotDotToken)
            {
                iSchemaLocation = -1;
                iTableLocation = 2;
            }
            else if (xoContext.List.Peek(1).ExpectedType == SyntaxKind.DotToken)
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
                    iSchemaLocation != -1 ? xoContext.List.Peek(iSchemaLocation).RawSQLText : String.Empty));

            oTable = new Symbol(new SyntaxToken(SyntaxKind.IdentifierTableSymbol, xoContext.List.Peek(iTableLocation).RawSQLText));

            // create the decorator obj
            oSchema.Add(oTable);
            oDatabase.Add(oSchema);

            // Pop the tokens
            xoContext.List.Pop(Math.Max(iSchemaLocation, iTableLocation) + 1);

            // Assign the alias
            ((Symbol)oTable).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);

            return oDatabase;
        }

        /// <summary>
        /// A factory method for interpreting what kind of Open Parenthesis construct
        /// we have here. 
        /// </summary>
        /// <param name="xoCurrentToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        private static SyntaxNode FactoryInterpretOpenParenthesisToken(ParsingContext xoContext)
        {
            // Given a (, Try and guess what type of () this is
            SyntaxNode oReturn;
            ISyntax xoCurrentToken = xoContext.List.Pop();

            // 1. ( SubQuery ) - (SELECT * FROM ..) svc
            // 4. SubQuery in Expression - Where X IN (SELECT Svc_IDNTY FROM svc)
           
            // First Keyword in is SELECT
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.SelectKeyword)
            {
                // Create SubQuery Symbol
                oReturn = FactoryCreateTable(xoContext);
            }
            // 2. ( Symbol List ) - (1, 2, 3) or (svc.MKT_PROD_CD, svc.SVC_STAT_CD) 
            // 3. ( Expressions ) - WHERE (X =Y) or (X <> Y AND X = 0)
            else
            {
                Boolean bCommaFound = false; // If you find any commas before the closing bracket
                Boolean bAllIdentifiersOrDots = true;

                for (int iIndex = 0; iIndex < xoContext.List.Count; iIndex++)
                {
                    ISyntax oLoopingNode = xoContext.List.Peek(iIndex);
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
                    oReturn = FactoryCreateColumn(xoContext);

                    // Ignore a trailing Close parenthesis
                    if (xoContext.List.Peek().ExpectedType == SyntaxKind.CloseParenthesisToken)
                    {
                        xoContext.List.Pop();
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

        private static SyntaxNode FactoryCreateCompoundJoin (ParsingContext xoContext)
        {
            // Break early if we got 1 join keyword
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.JoinKeyword)
            {
                // All Join nodes on their own become Inner joins
                SyntaxNode oJoinNode = new SyntaxNode(
                    xoContext.List.Peek(),
                    NodeStrategyFactory.FactoryCreateStrategy(xoContext.List.Pop().ExpectedType),
                    2);
                oJoinNode.ExpectedType = SyntaxKind.InnerJoinKeyword;
                return oJoinNode;
            }

            // Create the Join Node
            SyntaxNode oTemp = new SyntaxNode(xoContext.List.Peek(), 
                NodeStrategyFactory.FactoryCreateStrategy(xoContext.List.Pop().ExpectedType)); 

            // If the next node is actually an OUTER keyword
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.OuterKeyword)
            {
                // Construct a proper Join keyword with the type declared
                oTemp.RawSQLText += " " + xoContext.List.Pop().RawSQLText; // add the text (OUTER)
            }

            // If the next node is actually a Join
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.JoinKeyword)
            {
                // Construct a proper Join keyword with the type declared
                oTemp.RawSQLText += " " + xoContext.List.Pop().RawSQLText; // add the text (JOIN)
            }
            else
            {
                // Add an error
                oTemp.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8000, xoContext.List.Pop().RawSQLText, "JOIN"));

            }
            // Return the Join node
            return oTemp;
        }

        public static SyntaxNode FactoryCreateNot (ParsingContext xoContext)
        {
            int iMaxChildCount = 1;
            ISyntax oReturn = xoContext.List.Pop();
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.InKeyword)
            {
                oReturn.ExpectedType = SyntaxKind.NotInKeyword;
                oReturn.RawSQLText += " " + xoContext.List.Pop().RawSQLText;
                iMaxChildCount = 2;
            }
            else if (xoContext.List.Peek().ExpectedType == SyntaxKind.LikeKeyword)
            {
                oReturn.ExpectedType = SyntaxKind.NotLikeKeyword;
                oReturn.RawSQLText += " " + xoContext.List.Pop().RawSQLText;
                iMaxChildCount = 2;
            }

            return new SyntaxNode(oReturn, NodeStrategyFactory.FactoryCreateStrategy(oReturn.ExpectedType), iMaxChildCount);
        }

        private static SyntaxNode FactoryCreateColumnOrExpression (ParsingContext xoContext)
        {
            SyntaxKind eNextTokenKind = xoContext.List.Peek().ExpectedType;
            SyntaxNode oReturnNode;

            // Is any identifier or Expression (function/literal)
            if (SyntaxKindFacts.IsFunction(eNextTokenKind))
            {
                // COUNT (*)
                oReturnNode = FactoryCreateColumnFunction(xoContext);
            }
            else if (eNextTokenKind == SyntaxKind.OpenParenthesisToken)
            {
                oReturnNode = FactoryInterpretOpenParenthesisToken(xoContext);
            }
            else if (
                SyntaxKindFacts.IsIdentifier(eNextTokenKind) ||
                SyntaxKindFacts.IsLiteral(eNextTokenKind))
            {
                // Only Column List Nodes can create an Alias
                Boolean bIsAliasNeeded = xoContext.CurrentNode.ExpectedType == SyntaxKind.ColumnListNode && 
                    eNextTokenKind != SyntaxKind.StarToken; // Stars do not get alias'

                oReturnNode = FactoryCreateColumn(xoContext, bIsAliasNeeded);
            }
            else if (eNextTokenKind == SyntaxKind.CaseKeyword)
            {
                if (xoContext.CurrentNode.ExpectedType == SyntaxKind.ColumnListNode)
                {
                    return new Symbol(
                               xoContext.List.Peek(),
                               NodeStrategyFactory.FactoryCreateStrategy(xoContext.List.Pop().ExpectedType), -1);
                }
                else
                {
                    return new SyntaxNode(
                               xoContext.List.Peek(),
                               NodeStrategyFactory.FactoryCreateStrategy(xoContext.List.Pop().ExpectedType));
                }

            }
            else
            {
                oReturnNode = new SyntaxLeaf(xoContext.List.Pop());
            }

            // If we have a trailing || and we arent already using a bar bar
            if (xoContext.CurrentNode.ExpectedType != SyntaxKind.BarBarToken &&
                xoContext.List.Peek().ExpectedType == SyntaxKind.BarBarToken)
            {
                // Create a new bar bar node (to hold the children)
                SyntaxNode oBarNode = new SyntaxNode(xoContext.List.Pop());

                // Add the child 
                oBarNode.Add(oReturnNode);

                // return this collection
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
        private static SyntaxNode FactoryCreateColumn(ParsingContext xoContext, Boolean xbIsAliasNeeded = false)
        {
            ISyntax xoCurrentToken = xoContext.List.Peek();

            // A Symbol consists of multiple parts
            Symbol oTable;
            Symbol oColumn;

            // If this is a literal Column
            if (SyntaxKindFacts.IsLiteral(xoCurrentToken.ExpectedType))
            {
                oColumn = new Symbol(xoContext.List.Pop(), NodeStrategyFactory.NULL_STRATEGY);

                // Assign the alias
                if (xbIsAliasNeeded)
                {
                    oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);
                }

                return oColumn;
            }

            // Trailing item is . (table.column)
            if (xoContext.List.Peek(1).ExpectedType == SyntaxKind.DotToken)
            {
                oTable = new Symbol(xoCurrentToken, NodeStrategyFactory.NULL_STRATEGY);
                oColumn = new Symbol(xoContext.List.Peek(2)); // Grab the Column
                oTable.Add(oColumn);
                xoContext.List.Pop(3); // Skip over the next 2
            }
            // Standalone Column
            else
            {
                oTable = new Symbol(new SyntaxToken(SyntaxKind.IdentifierTableSymbol, String.Empty));
                oColumn = new Symbol(xoCurrentToken, NodeStrategyFactory.NULL_STRATEGY); // Grab the Column
                oTable.Add(oColumn);
                xoContext.List.Pop(); // Skip over the next 1
            }

            // Assign the alias
            if (xbIsAliasNeeded)
            {
                oColumn.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);
            }

            oColumn.ExpectedType = SyntaxKind.IdentifierColumnSymbol;
            oTable.ExpectedType = SyntaxKind.IdentifierTableSymbol;

            // Return the top level node
            return oTable;
        }

        private static SyntaxNode FactoryCreateColumnFunction(ParsingContext xoContext)
        {
            // generate the item
            Symbol oColumnExp = new Symbol(xoContext.List.Pop());

            // Consume Ahead
            oColumnExp.TryConsumeList(xoContext);

            // Assign the alias
            oColumnExp.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);

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
        public static String ScanAheadForAlias(SyntaxList xoList)
        {
            String sAlias = String.Empty;
            // SCAN AHEAD - To grab an alias if there is one
            if (xoList.HasTokensLeftToProcess())
            {
                ISyntax oNextNode = xoList.Peek();

                // Explicit
                if (oNextNode.ExpectedType == SyntaxKind.AsKeyword)
                {
                    // And the next node after that is an identifier
                    if (xoList.Peek(1).ExpectedType == SyntaxKind.IdentifierToken)
                    {
                        // Alias found
                        sAlias = xoList.Peek(1).RawSQLText;
                        xoList.Pop(2);
                    }
                }
                // Implicit
                else if (oNextNode.ExpectedType == SyntaxKind.IdentifierToken)
                {
                    // Alias found
                    sAlias = oNextNode.RawSQLText;
                    xoList.Pop();
                }
            }

            // Return the newly created table node
            return sAlias;
        }

       #endregion

    }
}
