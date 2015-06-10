using System;
using System.Collections.Generic;
using Turing.Factories;
using Turing.Parser;
using Turing.Syntax.Constructs.Exceptions;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Strategies
{
    class NodeStrategyFactory
    {
        #region Static Cache of Strategies

        // Set static Strategies to reduce memory usage
        public static readonly NodeStrategy NULL_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.NullTwoArgument,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild);

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

                    case SyntaxKind.CaseKeyword:
                        oReturnNode.EligibilityFn = CaseCanConsume;
                        break;
                    case SyntaxKind.WhenKeyword:
                        oReturnNode.EligibilityFn = WhenCanConsume;
                        break;
                    case SyntaxKind.ThenKeyword:
                    case SyntaxKind.ElseKeyword:
                        oReturnNode.EligibilityFn = ElseCanConsume;
                        break;

                    case SyntaxKind.IdentifierToken:
                    case SyntaxKind.IdentifierColumnSymbol:
                    case SyntaxKind.IdentifierTableSymbol:
                    case SyntaxKind.IdentifierSchemaSymbol:
                    case SyntaxKind.IdentifierDatabaseSymbol:
                        oReturnNode.EligibilityFn = NullTwoArgument;
                        break;

                    case SyntaxKind.BarBarToken:
                        oReturnNode.EligibilityFn = ConcatCanConsume;
                        break;

                    case SyntaxKind.IdentifierSubQuerySymbol:
                        oReturnNode.EligibilityFn = SubQueryCanConsumeNext;
                        break;

                    case SyntaxKind.ColumnListNode:
                        oReturnNode.EligibilityFn = SymbolListCanConsumeNext;
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

        // ?? TODO: Move to partial class
        #region Node Specific Methods

        #region COMMON

        /// <summary>
        /// Generic Table Symbol Creation from an Identifier
        /// </summary>
        /// <param name="xoContext.CurrentNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        private static SyntaxNode TableSymbolConvertToken(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                eKind == SyntaxKind.OpenParenthesisToken)
            {
                return SyntaxNodeFactory.FactoryCreateTable(xoContext);
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
        private static CanConsumeResult ExpressionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

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
            if (eResult == CanConsumeResult.Complete && xoContext.CurrentNode.Count != 1)
            {
                ResolutionGenerator.HandleIncompleteNode(xoContext);
            }

            return eResult;
        }

        #endregion

        #region Select

        private static CanConsumeResult SelectCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind oKind = xoContext.List.Peek().ExpectedType;

            // Only add core statements after the column list
            Boolean bIsCoreStatement = ((int)oKind >= (int)SyntaxKind.FromKeyword &&
                (int)oKind <= (int)SyntaxKind.HavingKeyword);

            // If a column list hasn't been initialised yet
            if (xoContext.CurrentNode.Count == 0 &&
                // And we have something that can be consumed by a column list
                (SyntaxKindFacts.IsIdentifierOrExpression(oKind) || 
                oKind == SyntaxKind.StarToken || oKind == SyntaxKind.BarBarToken))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            // Else we have a column list and we just found a core keyword
            else if (xoContext.CurrentNode.Count >= 1 && bIsCoreStatement)
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }
            // Else a terminating condition
            else if (SyntaxKindFacts.IsTerminatingNode(oKind))
            {
                // We want to remove the terminated node and quit
                xoContext.List.Pop();
                return CanConsumeResult.Complete;
            }
            // Clearly we have something we can't consume here
            else
            {
                return DefaultCanConsumeNext(xoContext);
            }
        }

        private static SyntaxNode SelectConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Build a Symbol Composite
            ISyntax oCurrentToken = xoContext.List.Peek();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifierOrExpression(oCurrentToken.ExpectedType) ||
                oCurrentToken.ExpectedType == SyntaxKind.BarBarToken ||
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

        private static CanConsumeResult FromCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                eKind == SyntaxKind.OpenParenthesisToken ||
                SyntaxKindFacts.IsJoinKeyword(eKind))
            {
                return CheckIfConsumptionIsAllowed(xoContext);
            }

            return DefaultCanConsumeNext(xoContext);
        }

        #endregion

        #region JOIN/ON

        private static CanConsumeResult JoinCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                eKind == SyntaxKind.OpenParenthesisToken ||
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
                if (xoContext.CurrentNode.Count != 3)
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

        private static CanConsumeResult SubQueryCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind oKind = xoContext.List.Peek().ExpectedType;

            // If we get a closing parenthesis
            if (xoContext.CurrentNode.ExpectedType == SyntaxKind.IdentifierSubQuerySymbol &&
                oKind == SyntaxKind.CloseParenthesisToken)
            {
                xoContext.List.Pop();

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
        private static CanConsumeResult FunctionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

            // If we get a closing parenthesis in a function
            if (eKind == SyntaxKind.CloseParenthesisToken)
            {
                // Drop it and complete the Function
                xoContext.List.Pop();
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

        private static CanConsumeResult BinaryExpressionCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

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
                if (xoContext.CurrentNode.Count != 2)
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                }

                return CanConsumeResult.Complete;
            }

            //
            return DefaultCanConsumeNext(xoContext);
        }
    
        private static SyntaxNode ExpressionConvertToken(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            if (xoContext.List.Peek().ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Create a unary expression
                return new SyntaxNode(xoContext.List.Pop(), NodeStrategyFactory.UNARY_EXPRESSION_STRATEGY); // UnaryExpression;);
            }
            else
            {
                // Let the base conversion figure out what it is
                return DefaultTryConsumeNext(xoContext);
            }
        }

        #endregion

        #region SymbolList

        private static Boolean EligibleColumnSymbol(SyntaxKind xeKind)
        {
            return
                SyntaxKindFacts.IsIdentifierOrExpression(xeKind) ||
                SyntaxKindFacts.IsFunction(xeKind) ||
                xeKind == SyntaxKind.StarToken ||
                xeKind == SyntaxKind.BarBarToken ||
                SyntaxKindFacts.IsArithmaticOperator(xeKind);
        }

        private static CanConsumeResult SymbolListCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind oKind = xoContext.List.Peek().ExpectedType;

            // Manually terminate if we have a keyword here
            if (SyntaxKindFacts.IsKeyword(oKind))
            {
                return CanConsumeResult.Complete;
            }
            else if (xoContext.CurrentNode.Count == 0 && // If this is the first item, it can consume an IDN
                EligibleColumnSymbol(oKind))
            {
                return CanConsumeResult.Consume;
            }
            // If we have some items
            else if (xoContext.CurrentNode.Count > 0 )
            {
                if (oKind == SyntaxKind.CommaToken)
                {
                    // Consume next
                    xoContext.List.Pop(); // drop the comma
                    return CanConsumeResult.Consume; // consume next
                }
                // Can consume conjunctive items
                else if (
                    oKind == SyntaxKind.BarBarToken ||
                    SyntaxKindFacts.IsArithmaticOperator(oKind)
                    )
                {
                    return CanConsumeResult.Consume;
                }
                // Goes to default
            }
       
            // Try default
            return DefaultCanConsumeNext(xoContext);
        }

        #endregion

        #region Expression List

        private static CanConsumeResult ConcatCanConsume(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Intermediate var
            SyntaxKind oKind = xoContext.List.Peek().ExpectedType;
            
            // Consume immediately
            if (xoContext.CurrentNode.Count <= 1)
            {
                if (SyntaxKindFacts.IsIdentifierOrExpression(oKind))
                {
                    return CanConsumeResult.Consume;
                }
                else
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                    return CanConsumeResult.Complete;
                }
            }
            // Get BARBAR, consume next
            else if (oKind == SyntaxKind.BarBarToken)
            {
                // Drop the bar bar and consume the next item
                xoContext.List.Pop();

                if (SyntaxKindFacts.IsIdentifierOrExpression(xoContext.List.Peek().ExpectedType))
                {
                    return CanConsumeResult.Consume;
                }
                else
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                    return CanConsumeResult.Complete;
                }
            }
            // Else close this object
            else
            {
                // Post consumption check
                if (xoContext.CurrentNode.Count <= 1)
                {
                    ResolutionGenerator.HandleIncompleteNode(xoContext);
                }

                return CanConsumeResult.Complete;
            }
        }

        #endregion

        #region Identifier

        private static CanConsumeResult IdentifierCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return CanConsumeResult.Complete;
        }

        #endregion

        #region CASE


        private static CanConsumeResult CaseCanConsume(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eNextKind = xoContext.List.Peek().ExpectedType;

            // Can consume When and Else. thats it
            if (eNextKind == SyntaxKind.WhenKeyword ||
                eNextKind == SyntaxKind.ElseKeyword)
            {
                return CanConsumeResult.Consume;
            }
            else if (eNextKind == SyntaxKind.EndKeyword)
            {
                xoContext.CurrentNode.Add(new SyntaxNode(xoContext.List.Pop()));

                // Only assign an alias if it is a symbol
                if (xoContext.CurrentNode.GetType() == typeof(Symbol))
                {
                    ((Symbol)xoContext.CurrentNode).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);
                }
                
                return CanConsumeResult.Complete;
            }
            else
            {
                // Improper closing of the CASE
                xoContext.CurrentNode.Add(new SyntaxNode(new SyntaxToken(SyntaxKind.EndKeyword, "END")));

                // Only assign an alias if it is a symbol
                if (xoContext.CurrentNode.GetType() == typeof(Symbol))
                {
                    ((Symbol)xoContext.CurrentNode).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoContext.List);
                }

                return CanConsumeResult.Complete;

                //return DefaultCanConsumeNext(xoContext);
            }
        }

        private static CanConsumeResult WhenCanConsume(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eNextKind = xoContext.List.Peek().ExpectedType;

            // Any Expression/Identifier
            if (SyntaxKindFacts.IsIdentifierOrExpression(eNextKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eNextKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eNextKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eNextKind) || // NOT
                SyntaxKindFacts.IsFunction(eNextKind) ||
                SyntaxKindFacts.IsArithmaticOperator(eNextKind) ||
                eNextKind == SyntaxKind.ThenKeyword)
            {
                return CanConsumeResult.Consume;
            }
            else
            {
                return CanConsumeResult.Complete;
            }
        }

        private static CanConsumeResult ElseCanConsume(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eNextKind = xoContext.List.Peek().ExpectedType;

            // Any Expression/Identifier
            if (SyntaxKindFacts.IsIdentifierOrExpression(eNextKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eNextKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eNextKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eNextKind) || // NOT
                SyntaxKindFacts.IsFunction(eNextKind) ||
                SyntaxKindFacts.IsArithmaticOperator(eNextKind))
            {
                return CanConsumeResult.Consume;
            }
            else
            {
                return CanConsumeResult.Complete;
            }
        }

        #endregion

        #region Default Methods

        private static Boolean NodeIsOpenParenthesis(SyntaxNode oNode)
        {
            return
                oNode.ExpectedType == SyntaxKind.OpenParenthesisToken ||
                oNode.RawSQLText.Equals("(");
        } 

        /// <summary>
        /// Returns true if the next node is something this node can interpret
        /// and returns false if it cannot do anything with the next node
        /// </summary>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        private static CanConsumeResult DefaultCanConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            SyntaxKind eKind = xoContext.List.Peek().ExpectedType;

            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (eKind == SyntaxKind.CloseParenthesisToken)
            {
                // We have an open paren node, remove the token and close
                if (NodeIsOpenParenthesis(xoContext.CurrentNode))
                {
                    xoContext.List.Pop();
                    // Helps us get back to the appropriate Node
                    return CanConsumeResult.Complete;
                }
                else
                {
                    // Scan up to see if there is an open parenthesis.
                    SyntaxNode oOpenParenParent = xoContext.CurrentNode.FindFirstParent(
                        NodeIsOpenParenthesis);

                    if (oOpenParenParent != null)
                    {
                        // Helps us get back to the appropriate Node
                        return CanConsumeResult.Complete;
                    }
                    else
                    {
                        // Invalid Closing parenthesis here
                        xoContext.CurrentNode.Add(new SkippedNode(xoContext.List.Pop())); // Add as excluded node
                        // keep processing
                        return CanConsumeResult.Skip;
                    }
                }

            }
            // Terminate if we find an eof of any sort or we are full
            else if (SyntaxKindFacts.IsTerminatingNode(eKind) || 
                xoContext.CurrentNode.IsFull())
            {
                return CanConsumeResult.Complete;
            }
            else
            {
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
        private static CanConsumeResult CheckIfConsumptionIsAllowed(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            Boolean bIsCompoundNotItem = xoContext.List != null ?
                xoContext.List.Peek().ExpectedType == SyntaxKind.NotKeyword &&
                (xoContext.List.Peek(1).ExpectedType == SyntaxKind.InKeyword ||
                xoContext.List.Peek(1).ExpectedType == SyntaxKind.LikeKeyword) : false;

            Boolean bIsBinaryOperator =
                bIsCompoundNotItem ||
                SyntaxKindFacts.IsBinaryConstruct(xoContext.List.Peek().ExpectedType);

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
        private static SyntaxNode DefaultTryConsumeNext(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoContext);
        }

        /// <summary>
        /// Postprocessing method that can be overriden if some activity needs to be
        /// done immediately after a node is constructed
        /// </summary>
        /// <param name="xoNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        private static Boolean DefaultAddChild(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            // Is not a node, cannot be added
            if (!xoContext.List.Peek().IsNode())
            {
                return false;
            }

            SyntaxNode oNewNode = (SyntaxNode)xoContext.List.Pop();
            
            // Do preconsumption here
            if (SyntaxKindFacts.IsBinaryConstruct(oNewNode.ExpectedType))
            {
                // If there is nothing to preconsume
                if (xoContext.CurrentNode.Count == 0)
                {
                    // Else we have an error to fix
                    ResolutionGenerator.HandlePreconsumptionError(new ParsingContext(oNewNode, xoContext.List));
                }
                // If there is something to preconsume
                else
                {
                    int iSiblingPosition = xoContext.CurrentNode.Count - 1;
                    SyntaxNode oPrevSibling = xoContext.CurrentNode[iSiblingPosition];

                    // Put the previous sibling back on the List to be consumed
                    xoContext.List.Insert(oPrevSibling);

                    // Check the eligibility of the previous node
                    CanConsumeResult eEligibility = oNewNode.CanConsumeNode(
                        new ParsingContext(oNewNode, xoContext.List), true);

                    if (eEligibility == CanConsumeResult.Consume)
                    {
                        // Assign the parent 
                        oNewNode.Parent = xoContext.CurrentNode;

                        // Pull off the last node from the parent
                        oNewNode.Add((SyntaxNode)xoContext.List.Pop());

                        // Remove it too
                        xoContext.CurrentNode.RemoveAt(iSiblingPosition);
                    }
                    else
                    {
                        // Else we have an error to fix
                        ResolutionGenerator.HandlePreconsumptionError(new ParsingContext(oNewNode, xoContext.List));
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
                xoContext.CurrentNode.Add(oNewNode);
                // 2. Depth first traversal from the child
                if (oNewNode.TryConsumeFromContext(xoContext))
                {
                    // If it successfully consumed something
                }
                return true;
            }
        }

        #endregion

        #region Null Methods

        private static CanConsumeResult NullTwoArgument(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return CanConsumeResult.Complete;
        }

        #endregion

        #endregion Node Methods

    }
}
