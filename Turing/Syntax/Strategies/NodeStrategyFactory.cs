using System;
using System.Collections.Generic;
using Turing.Factories;
using Turing.Parser;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Strategies
{
    class NodeStrategyFactory
    {
        #region Static Cache of Strategies

        // Set static Strategies to reduce memory
        public static readonly NodeStrategy DEFAULT_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.DefaultCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy NULL_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.NullTwoArgument,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.NullThreeArgument);

        //public static readonly NodeStrategy SYMBOL_LIST_STRATEGY = new NodeStrategy(
        //    NodeStrategyFactory.SymbolListCanConsumeNext,
        //    NodeStrategyFactory.SymbolListConsumeNext,
        //    NodeStrategyFactory.DefaultAddChild);

        public static readonly NodeStrategy UNARY_EXPRESSION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.ExpressionCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild);

        #endregion

        #region Core Factory Method

        public static Dictionary<SyntaxKind, NodeStrategy> dsStrategyCache = new Dictionary<SyntaxKind, NodeStrategy>()
        {
        };

        public static NodeStrategy FactoryCreateStrategy(SyntaxKind xeKind)
        {
            if (dsStrategyCache.ContainsKey(xeKind))
            {
                return dsStrategyCache[xeKind];
            }
            else
            {
                // Set everything to default
                NodeStrategy oReturnNode = new NodeStrategy(
                    DefaultCanConsumeNext,
                    DefaultTryConsumeNext,
                    DefaultAddChild); // Default

                // Explicit Strategies
                switch (xeKind)
                {
                    case SyntaxKind.SelectKeyword:
                        oReturnNode.EligibilityFn = SelectCanConsumeNext;
                        oReturnNode.TryConsumeNextFn = SelectConsumeNext;
                        break;
                    case SyntaxKind.FromKeyword:
                        oReturnNode.EligibilityFn = FromCanConsumeNext;
                        oReturnNode.TryConsumeNextFn = TableSymbolConvertToken;
                        break;
                    case SyntaxKind.WhereKeyword:
                    case SyntaxKind.OnKeyword:
                    case SyntaxKind.NotKeyword:
                        oReturnNode.EligibilityFn = ExpressionCanConsumeNext;
                        break;

                    case SyntaxKind.IdentifierToken:
                    case SyntaxKind.IdentifierColumnSymbol:
                    case SyntaxKind.IdentifierTableSymbol:
                    case SyntaxKind.IdentifierSchemaSymbol:
                    case SyntaxKind.IdentifierDatabaseSymbol:
                        oReturnNode.EligibilityFn = NullTwoArgument;
                        break;

                    case SyntaxKind.IdentifierSubQuerySymbol:
                        oReturnNode.EligibilityFn = SubQueryCanConsumeNext;
                        break;

                    case SyntaxKind.ColumnListNode:
                        oReturnNode.EligibilityFn = SymbolListCanConsumeNext;
                        oReturnNode.TryConsumeNextFn = SymbolListConsumeNext;
                        break;
                }

                // Other Explicit conversions can be performed here (saves on writing every kind above)
                if (SyntaxKindFacts.IsFunction(xeKind))
                {
                    oReturnNode.EligibilityFn = FunctionCanConsumeNext;
                }
                // Join Keyword
                else if (SyntaxKindFacts.IsJoinKeyword(xeKind))
                {
                    oReturnNode.EligibilityFn = JoinCanConsumeNext;
                    oReturnNode.TryConsumeNextFn = TableSymbolConvertToken;
                }
                // Arithmatic/Conditional/Adjunct operators
                else if (
                    SyntaxKindFacts.IsAdjunctConditionalOperator(xeKind) ||
                    SyntaxKindFacts.IsArithmaticOperator(xeKind) ||
                    SyntaxKindFacts.IsConditionalOperator(xeKind))
                {
                    oReturnNode.EligibilityFn = BinaryExpressionCanConsumeNext;
                }
                else if (SyntaxKindFacts.IsLiteral(xeKind))
                {
                    oReturnNode.EligibilityFn = NullTwoArgument;
                }

                // Cache it
                dsStrategyCache.Add(xeKind, oReturnNode);

                // return it
                return oReturnNode;
            }
        }

        #endregion

        #region Node Specific Methods

        #region COMMON

        /// <summary>
        /// Generic Table Symbol Creation from an Identifier
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static SyntaxNode TableSymbolConvertToken(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxToken xoCurrentToken = xoContext.List.PeekToken();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(xoCurrentToken.ExpectedType))
            {
                return SyntaxNodeFactory.FactoryCreateTable(xoContext.List);
            }
            else
            {
                // Everything else
                return DefaultTryConsumeNext(xoContext);
            }
        }

        /// <summary>
        /// Used by WHERE and ON
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static CanConsumeResult ExpressionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.NextItemKind();

            if (
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsFunction(eKind) ||
                SyntaxKindFacts.IsArithmaticOperator(eKind))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }


            CanConsumeResult eResult = DefaultCanConsumeNext(xoContext);

            // Post execution check
            if (eResult == CanConsumeResult.Complete && xoContext.CurrentNode.Children.Count != 1)
            {
                ResolutionGenerator.HandleIncompleteNode(xoContext);
            }

            return eResult;
        }

        #endregion

        #region Select

        public static CanConsumeResult SelectCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind oKind = xoContext.NextItemKind();

            // Only add core statements after the column list
            Boolean bIsCoreStatement = ((int)oKind >= (int)SyntaxKind.FromKeyword &&
                (int)oKind <= (int)SyntaxKind.HavingKeyword);

            // Enforces only 1 column list is initialised
            Boolean bTryingToAddDuplicateColumnList = SyntaxKindFacts.IsIdentifier(oKind) &&
                    xoContext.CurrentNode.Children.Exists((oNode) => oNode.ExpectedType == SyntaxKind.ColumnListNode);

            if (!bTryingToAddDuplicateColumnList && 
                (SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken ||
                bIsCoreStatement))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            else if (SyntaxKindFacts.IsTerminatingNode(oKind))
            {
                // We want to consume the terminated node and quit
                xoContext.CurrentNode.AddChild(DefaultTryConsumeNext(xoContext));
                return CanConsumeResult.Complete;
            }
            // Clearly we have something we can't consume here
            else
            {
                return CanConsumeResult.Complete;
            }
        }

        public static SyntaxNode SelectConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Build a Symbol Composite
            SyntaxToken oCurrentToken = xoContext.List.PeekToken();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(oCurrentToken.ExpectedType) ||
                SyntaxKindFacts.IsLiteral(oCurrentToken.ExpectedType) ||
                SyntaxKindFacts.IsFunction(oCurrentToken.ExpectedType) ||
                oCurrentToken.ExpectedType == SyntaxKind.StarToken) // * in Column is allowed
            {
                // Initialise a list
                SymbolList oList = new SymbolList();

                // generate a symbol list (which will consume anything else that is a column)
                return oList;
            }

            // Default to using the original conversion
            return DefaultTryConsumeNext(xoContext);
        }

        #endregion

        #region FROM

        public static CanConsumeResult FromCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.NextItemKind();
            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                SyntaxKindFacts.IsJoinKeyword(eKind))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }

            return DefaultCanConsumeNext(xoContext);
        }

        #endregion

        #region JOIN/ON

        public static CanConsumeResult JoinCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.NextItemKind();

            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                eKind == SyntaxKind.OnKeyword ||
                (xbIsPreconsumption && SyntaxKindFacts.IsJoinKeyword(eKind)) // Allow preconsumption to use JOIN keywods
                )
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            // If we got another keyword (that we cant process)
            else if (
                SyntaxKindFacts.IsTerminatingNode(eKind) ||
                SyntaxKindFacts.IsKeyword(eKind) || 
                eKind == SyntaxKind.CloseParenthesisToken)
            {
                // Post execution check
                if (xoContext.CurrentNode.Children.Count != 3)
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                }

                return CanConsumeResult.Complete;
            }

            CanConsumeResult eResult = DefaultCanConsumeNext(xoContext);



            return DefaultCanConsumeNext(xoContext);
        }

        #endregion

        #region SubQuery

        public static CanConsumeResult SubQueryCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind oKind = xoContext.NextItemKind();

            // If we get a closing parenthesis
            if (xoContext.CurrentNode.ExpectedType == SyntaxKind.IdentifierSubQuerySymbol &&
                oKind == SyntaxKind.CloseParenthesisToken)
            {
                xoContext.List.PopToken();

                // Just add an Alias
                ((Symbol)xoContext.CurrentNode).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);

                return CanConsumeResult.Complete;
            }
            else if (oKind == SyntaxKind.SelectKeyword)
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            else
            {
                return DefaultCanConsumeNext(xoContext);
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Finish up as soon as we come across a Close Parenthesis
        /// </summary>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static CanConsumeResult FunctionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.NextItemKind();

            // If we get a closing parenthesis in a function
            if (eKind == SyntaxKind.CloseParenthesisToken)
            {
                // Drop it and complete the Function
                xoContext.List.PopToken();
                return CanConsumeResult.Complete;
            }
            else if (SyntaxKindFacts.IsIdentifierOrExpression(eKind) ||
                eKind == SyntaxKind.StarToken)
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }

            return DefaultCanConsumeNext(xoContext);
        }

        #endregion

        #region Expressions

        public static CanConsumeResult BinaryExpressionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind eKind = xoContext.NextItemKind();

            // If we have something we are actually allowed to consume
            if ((xbIsPreconsumption && SyntaxKindFacts.IsAdjunctConditionalOperator(eKind)) || // Allow AND/OR in preconsump
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsArithmaticOperator(eKind))
            {
                CanConsumeResult eResult = CheckIfConsumptionIsAllowed(xoContext);

                switch (eResult)
                {
                    // Possible erroroneous
                    case CanConsumeResult.Unknown:

                    // Definitely finished
                    case CanConsumeResult.Complete:
                        // Perform final checks
                        break;
                    
                    // Break immediately
                    case CanConsumeResult.Skip:
                    case CanConsumeResult.Consume:
                        return eResult;
                }
            }

            // Closing condition
            if (xoContext.CurrentNode.IsFull() ||
                SyntaxKindFacts.IsTerminatingNode(eKind) ||
                SyntaxKindFacts.IsKeyword(eKind) || 
                eKind == SyntaxKind.CloseParenthesisToken)
            {
                // Post execution check
                if (xoContext.CurrentNode.Children.Count != 2)
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                }

                return CanConsumeResult.Complete;
            }

            //
            return DefaultCanConsumeNext(xoContext);
        }


    

        public static SyntaxNode ExpressionConvertToken(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            if (xoContext.NextItemKind() == SyntaxKind.OpenParenthesisToken)
            {
                // Create a unary expression
                return new SyntaxNode(xoContext.List.PopToken(), NodeStrategyFactory.UNARY_EXPRESSION_STRATEGY); // UnaryExpression;);
            }
            else
            {
                // Let the base conversion figure out what it is
                return DefaultTryConsumeNext(xoContext);
            }
        }

        #endregion

        #region SymbolList

        public static CanConsumeResult SymbolListCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind oKind = xoContext.NextItemKind();

            // If we get a comma, just drop it
            if (oKind == SyntaxKind.CommaToken)
            {
                xoContext.List.PopToken();
                return CanConsumeResult.Skip;
            }
            else if (
                SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                SyntaxKindFacts.IsFunction(oKind) ||
                oKind == SyntaxKind.StarToken ||
                SyntaxKindFacts.IsArithmaticOperator(oKind))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            // Terminate manually if we find a keyword
            else if(SyntaxKindFacts.IsKeyword(oKind))
            {
                return CanConsumeResult.Complete;
            }

            // Try convert
            return DefaultCanConsumeNext(xoContext);
        }


        public static SyntaxNode SymbolListConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            if (SyntaxKindFacts.IsLiteral(xoContext.NextItemKind()))
            {
                return SyntaxNodeFactory.FactoryCreateColumn(xoContext.List);
            }
            else
            {
                // Let the base conversion figure out what it is
                return DefaultTryConsumeNext(xoContext);
            }
        }

        #endregion

        #region Expression List


        public static CanConsumeResult ExpressionListCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind oKind = xoContext.NextItemKind();

            // If we get a comma, just drop it
            if (oKind == SyntaxKind.BarBarToken)
            {
                // Drop the bar bar and consume the next item
                xoContext.List.PopToken();
                return CanConsumeResult.Consume;
            }
            else
            {
                // Try convert
                return DefaultCanConsumeNext(xoContext);
            }
        }


        public static SyntaxNode ExpressionListConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            if (SyntaxKindFacts.IsLiteral(xoContext.NextItemKind()))
            {
                return SyntaxNodeFactory.FactoryCreateColumn(xoContext.List);
            }
            else
            {
                // Let the base conversion figure out what it is
                return DefaultTryConsumeNext(xoContext);
            }
        }

  
        #endregion

        #region Identifier

        public static CanConsumeResult IdentifierCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return CanConsumeResult.Complete;
        }

        #endregion

        #region Default Methods

        /// <summary>
        /// Returns true if the next node is something this node can interpret
        /// and returns false if it cannot do anything with the next node
        /// </summary>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static CanConsumeResult DefaultCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.NextItemKind();
            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if ((xoContext.CurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken ||
                xoContext.CurrentNode.RawSQLText.Equals("(")) &&
                eKind == SyntaxKind.CloseParenthesisToken)
            {
                xoContext.List.PopToken();
                return CanConsumeResult.Complete;
            }
            // Terminate if we find an eof of any sort or we are full
            else if (SyntaxKindFacts.IsTerminatingNode(eKind) || 
                xoContext.CurrentNode.IsFull())
            {
                return CanConsumeResult.Complete;
            }
            else
            {
                // ?? TODO: Stop everything from coming here and implement an Unknown type?
                // Unknown, Missing?
                return CanConsumeResult.Unknown;
            }
        }

        /// <summary>
        /// Before returning a Consume result, we should do a few more checks
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static CanConsumeResult CheckIfConsumptionIsAllowed(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            Boolean bIsCompoundNotItem = xoContext.List != null ?
                xoContext.List.PeekToken().ExpectedType == SyntaxKind.NotKeyword &&
                (xoContext.List.PeekToken(1).ExpectedType == SyntaxKind.InKeyword ||
                xoContext.List.PeekToken(1).ExpectedType == SyntaxKind.LikeKeyword) : false;

            Boolean bIsBinaryOperator =
                bIsCompoundNotItem ||
                SyntaxKindFacts.IsBinaryConstruct(xoContext.NextItemKind());

            // Only forcibly add a construct after we are full IF it is a binary construct
            if (xoContext.CurrentNode.IsFull() && 
                !bIsBinaryOperator)
            {
                // We also find most of our problematic nodes here
                // And orphaned nodes

                return CanConsumeResult.Complete;
            }

            return CanConsumeResult.Consume;
        }

        /// <summary>
        /// Provides context sensitive generation of a new Node
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static SyntaxNode DefaultTryConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoContext.CurrentNode, xoContext.List);
        }

        /// <summary>
        /// Postprocessing method that can be overriden if some activity needs to be
        /// done immediately after a node is constructed
        /// </summary>
        /// <param name="xoNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        public static Boolean DefaultAddChild(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxNode oNewNode = xoContext.NewNode;
            
            // Do preconsumption here
            if (SyntaxKindFacts.IsBinaryConstruct(oNewNode.ExpectedType))
            {
                // If there is nothing to preconsume
                if (xoContext.CurrentNode.Children.Count == 0)
                {
                    // Else we have an error to fix
                    ResolutionGenerator.HandlePreconsumptionError(new ParsingContext(oNewNode, null, xoContext.List));
                }
                // If there is something to preconsume
                else
                {
                    int iSiblingPosition = xoContext.CurrentNode.Children.Count - 1;
                    SyntaxNode oPrevSibling = xoContext.CurrentNode.Children[iSiblingPosition];
                    
                    // Check the eligibility of the previous node
                    CanConsumeResult eEligibility = oNewNode.CanConsumeNode(
                        new ParsingContext(oNewNode, oPrevSibling, null), true);

                    if (eEligibility == CanConsumeResult.Consume)
                    {
                        // Assign the parent 
                        oNewNode.Parent = xoContext.CurrentNode;

                        // Pull off the last node from the parent
                        oNewNode.AddChild(oPrevSibling);

                        // Remove it too
                        xoContext.CurrentNode.Children.RemoveAt(iSiblingPosition);
                    }
                    else
                    {
                        // Else we have an error to fix
                        ResolutionGenerator.HandlePreconsumptionError(new ParsingContext(oNewNode, null, xoContext.List));
                    }
                }
            }

            // If it is full
            if (xoContext.CurrentNode.IsFull())
            {
                return false;
            }
            else
            {
                // Add the child
                xoContext.CurrentNode.AddChild(oNewNode);
                // 2. Depth first traversal from the child
                if (oNewNode.TryConsumeList(xoContext.List))
                {
                    // If it successfully consumed something
                }
                return true;
            }
        }

        #endregion

        #region Null Methods

        public static CanConsumeResult NullTwoArgument(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return CanConsumeResult.Complete;
        }

        public static Boolean NullThreeArgument(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return false;
        }

        #endregion

        #endregion Node Methods

    }
}
