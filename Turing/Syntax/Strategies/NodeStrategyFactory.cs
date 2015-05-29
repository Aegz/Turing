using System;
using System.Collections.Generic;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Expressions;
using Turing.Syntax.Constructs.Symbols;
using Turing.Syntax.Constructs.Symbols.Collections;

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

        public static readonly NodeStrategy SYMBOL_LIST_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.SymbolListCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy BINARY_EXPRESSION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.BinaryExpressionCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy UNARY_EXPRESSION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.ExpressionCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy JOIN_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.JoinCanConsumeNext,
            NodeStrategyFactory.TableSymbolConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy SUBQUERY_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.SubQueryCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy FUNCTION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.FunctionCanConsumeNext,
            NodeStrategyFactory.DefaultTryConsumeNext,
            NodeStrategyFactory.DefaultAddChild); // Default

        #endregion

        #region Core Factory Method

        public static Dictionary<SyntaxKind, NodeStrategy> dsStrategyCache = new Dictionary<SyntaxKind, NodeStrategy>()
        {
        };

        public static NodeStrategy FactoryCreateStrategy (SyntaxKind xeKind)
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
                    NodeStrategyFactory.DefaultTryConsumeNext,
                    NodeStrategyFactory.DefaultAddChild); // Default

                // Create the strategy
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
                        oReturnNode.EligibilityFn = ExpressionCanConsumeNext;
                        //oReturnNode.ConvertTokenFn = ColumnSymbolConvertToken;
                        break;
                    case SyntaxKind.IdentifierToken:
                        oReturnNode.EligibilityFn = IdentifierCanConsumeNext;
                        break;

                    // JOIN
                    case SyntaxKind.JoinKeyword:
                    case SyntaxKind.InnerJoinKeyword:
                    case SyntaxKind.OuterKeyword:
                    case SyntaxKind.LeftJoinKeyword:
                    case SyntaxKind.RightJoinKeyword:
                    case SyntaxKind.CrossJoinKeyword:
                        oReturnNode.EligibilityFn = JoinCanConsumeNext;
                        oReturnNode.TryConsumeNextFn = TableSymbolConvertToken;
                        break;

                    case SyntaxKind.EqualsToken:
                    case SyntaxKind.GreaterThanOrEqualToken:
                    case SyntaxKind.GreaterThanToken:
                    case SyntaxKind.LessThanOrEqualToToken:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.DiamondToken:

                    case SyntaxKind.PlusToken:
                    case SyntaxKind.MinusToken:
                    case SyntaxKind.StarToken:
                    case SyntaxKind.SlashToken:

                    case SyntaxKind.AndKeyword:
                    case SyntaxKind.OrKeyword:
                        oReturnNode.EligibilityFn = BinaryExpressionCanConsumeNext;
                        break;
                }

                // Store it
                dsStrategyCache.Add(xeKind, oReturnNode);

                // return it
                return oReturnNode;
            }
        }

        #endregion

        #region Node Specific Methods

        #region COMMON

        /// <summary>
        /// Generic Column Symbol Creation from an Identifier
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode ColumnSymbolConvertToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Default to using the original conversion
            return DefaultTryConsumeNext(xoCurrentNode, xoList);
        }

        /// <summary>
        /// Generic Table Symbol Creation from an Identifier
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode TableSymbolConvertToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxToken xoCurrentToken = xoList.PeekToken();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(xoCurrentToken.ExpectedType)) 
            {
                return SyntaxNodeFactory.FactoryCreateTable(xoList);
            }
            else
            {
                // Everything else
                return DefaultTryConsumeNext(xoCurrentNode, xoList);
            }
        }

        /// <summary>
        /// Used by WHERE and ON
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static CanConsumeResult ExpressionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eKind = xoList.PeekToken().ExpectedType;

            if(
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsFunction(eKind) ||
                SyntaxKindFacts.IsArithmaticOperator(eKind))
            {
                return CanConsumeResult.Consume;
            }
            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }

        #endregion

        #region Select

        public static SyntaxNode SelectConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {

            // Build a Symbol Composite
            SyntaxToken oCurrentToken = xoList.PeekToken();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(oCurrentToken.ExpectedType) ||
                SyntaxKindFacts.IsFunction(oCurrentToken.ExpectedType) ||
                oCurrentToken.ExpectedType == SyntaxKind.StarToken) // * in Column is allowed
            {
                // Initialise a list
                SymbolList oList = new SymbolList();

                // generate a symbol list (which will consume anything else that is a column)
                return oList;
            }

            // Default to using the original conversion
            return DefaultTryConsumeNext(xoCurrentNode, xoList);
        }

        public static CanConsumeResult SelectCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            if (
                SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken ||
                ((int)oKind >= (int)SyntaxKind.FromKeyword &&
                (int)oKind <= (int)SyntaxKind.HavingKeyword))
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }

        #endregion

        #region FROM

        public static CanConsumeResult FromCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;
            if (SyntaxKindFacts.IsIdentifier(oKind) ||
                SyntaxKindFacts.IsJoinKeyword(oKind))
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);                
        }

        #endregion

        #region JOIN/ON

        public static CanConsumeResult JoinCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we need to preconsume
            if (xoCurrentNode.Children.Count == 0)
            {
                // Consume the last sibling that fits the criteria
                if (xoCurrentNode.TryConsumePreviousSibling(
                (oKind) =>
                    SyntaxKindFacts.IsIdentifier(oKind) || // Identifier/(Table)
                    SyntaxKindFacts.IsJoinKeyword(oKind))
                )
                {
                    // Consumed child successfully
                }
                else
                {
                    // Error
                    // Insert dummy Child
                    xoCurrentNode.AddChild(new SyntaxNode(new SyntaxToken(SyntaxKind.IdentifierToken, "MISSING IDN"), NULL_STRATEGY));
                }
            }

            SyntaxKind eKind = xoList.PeekToken().ExpectedType;
            if (SyntaxKindFacts.IsIdentifier(eKind) ||
                eKind == SyntaxKind.OnKeyword)
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }

        public static Boolean JoinPostProcess(SyntaxNode xoCurrentNode, SyntaxNode xoNewNode, SyntaxTokenList xoList)
        {

            // Return true here
            return DefaultAddChild(xoCurrentNode, xoNewNode, xoList);
        }

        #endregion

        #region SubQuery

        public static CanConsumeResult SubQueryCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            // If we get a closing parenthesis
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                oKind == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                // Just add an Alias
                ((Symbol)xoCurrentNode).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return CanConsumeResult.Complete;
            }
            else if (oKind == SyntaxKind.SelectKeyword)
            {
                return CanConsumeResult.Consume;
            }
            else
            {
                return DefaultCanConsumeNext(xoCurrentNode, xoList);
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Finish up as soon as we come across a Close Parenthesis
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static CanConsumeResult FunctionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;
            // If we get a comma, just drop it
            if (oKind == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Complete;
            }
            else if (SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken)
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }

        #endregion

        #region Expressions

        public static CanConsumeResult BinaryExpressionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        { 
            // If we need to preconsume
            if (xoCurrentNode.Children.Count == 0)
            {
                // Consume the last sibling that fits the criteria
                if (xoCurrentNode.TryConsumePreviousSibling(
                (oKind) =>
                    SyntaxKindFacts.IsConditionalOperator(oKind) ||         // conditional operators ==, >=
                    SyntaxKindFacts.IsAdjunctConditionalOperator(oKind) ||  // and/or 
                    SyntaxKindFacts.IsIdentifierOrExpression(oKind)         // Expressions
                ))
                {
                    // Consumed child successfully
                    //return CanConsumeResult.Skip;
                }
                else
                {
                    // Error
                    // Insert dummy Child
                    xoCurrentNode.AddChild(new SyntaxNode(new SyntaxToken(SyntaxKind.IdentifierToken, "MISSING IDN/Literal/Expression"), NULL_STRATEGY));
                }
            }

            SyntaxKind eKind = xoList.PeekToken().ExpectedType;

            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                eKind == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Complete;
            }
            else if (
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                //SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsArithmaticOperator(eKind))
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }


        public static Boolean BinaryExpressionPostProcess(SyntaxNode xoCurrentNode, SyntaxNode xoNewNode, SyntaxTokenList xoList)
        {
            //// If we need to preconsume
            //if (xoCurrentNode.Children.Count == 0)
            //{
            //    // Consume the last sibling that fits the criteria
            //    if (xoCurrentNode.TryConsumePreviousSibling(
            //    (oKind) =>
            //        SyntaxKindFacts.IsConditionalOperator(oKind) ||         // conditional operators ==, >=
            //        SyntaxKindFacts.IsAdjunctConditionalOperator(oKind) ||  // and/or
            //                                                                //SyntaxKindFacts.IsLiteral(oKind) ||                   // Literals
            //        SyntaxKindFacts.IsIdentifierOrExpression(oKind)         // Expressions
            //    ))
            //    {
            //        // Consumed child successfully
            //    }
            //    else
            //    {
            //        // Error
            //        // Insert dummy Child
            //        xoCurrentNode.AddChild(new SyntaxNode(new SyntaxToken(SyntaxKind.IdentifierToken, "MISSING IDN/Literal/Expression"), NULL_STRATEGY));
            //    }
            //}

            // Return true here
            return DefaultAddChild(xoCurrentNode, xoNewNode, xoList);
        }


        public static CanConsumeResult UnaryExpressionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eKind = xoList.PeekToken().ExpectedType;

            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                eKind == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Complete;
            }
            else if (
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsArithmaticOperator(eKind))
            {
                return CanConsumeResult.Consume;
            }

            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }


        public static SyntaxNode ExpressionConvertToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we have an identifier on its own
            if (SyntaxKindFacts.IsIdentifier(xoList.PeekToken().ExpectedType))
            {
                // only build columns
                return ColumnSymbolConvertToken(xoCurrentNode, xoList);
            }
            else if (xoList.PeekToken().ExpectedType == SyntaxKind.OpenParenthesisToken)
            {
                // Create a unary expression
                return new UnaryExpression(xoList.PopToken());
            }
            else
            {
                // Let the base conversion figure out what it is
                return DefaultTryConsumeNext(xoCurrentNode, xoList);
            }
        }

        #endregion

        #region SymbolList

        public static CanConsumeResult SymbolListCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Intermediate var
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                oKind == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Complete;
            }
            // If we get a comma, just drop it
            else if (oKind == SyntaxKind.CommaToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Skip;
            }
            else if (
                SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                SyntaxKindFacts.IsFunction(oKind) ||
                oKind == SyntaxKind.StarToken ||
                SyntaxKindFacts.IsArithmaticOperator(oKind))
            {
                return CanConsumeResult.Consume;
            }

            // Try convert
            return DefaultCanConsumeNext(xoCurrentNode, xoList);
        }

        #endregion

        #region Identifier

        public static CanConsumeResult IdentifierCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            return CanConsumeResult.Complete;
        }

        #endregion

        #region Default Methods

        /// <summary>
        /// Returns true if the next node is something this node can interpret
        /// and returns false if it cannot do anything with the next node
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static CanConsumeResult DefaultCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                return CanConsumeResult.Complete;
            }
            // Terminate if we find an eof of any sort
            else if (SyntaxKindFacts.IsTerminatingNode(xoList.PeekToken().ExpectedType))
            {
                return CanConsumeResult.Complete;
            }
            else
            {
                return CanConsumeResult.Complete;
            }
        }

   
        /// <summary>
        /// Provides context sensitive generation of a new Node
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode DefaultTryConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoCurrentNode, xoList);
        }

        /// <summary>
        /// Postprocessing method that can be overriden if some activity needs to be
        /// done immediately after a node is constructed
        /// </summary>
        /// <param name="xoNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean DefaultAddChild(SyntaxNode xoCurrentNode, SyntaxNode xoNode, SyntaxTokenList xoList)
        {
            // Consume Sibling here

            // Add the child to this nodes children
            if (xoCurrentNode.AddChild(xoNode))
            {
                // 2. Depth first traversal from the child
                if (xoNode.TryConsumeList(xoList))
                {
                    // If it successfully consumed something
                }
                return true;
            }
            else
            {
                // Could not add node, add a message
                xoCurrentNode.Comments.Add(ErrorMessageLibrary.GetErrorMessage(
                    8010, 
                    ReasonMessageLibrary.GetReasonMessage(8010),
                    xoCurrentNode.RawSQLText,
                    xoNode.RawSQLText));

                return false;
            }
        }

        #endregion

        #region Null Methods

        public static CanConsumeResult NullTwoArgument(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            return CanConsumeResult.Complete;
        }
    
        public static Boolean NullThreeArgument(SyntaxNode xoCurrentNode, SyntaxNode xoNewNode, SyntaxTokenList xoList)
        {
            return false;
        }

        #endregion
        
        #endregion Node Methods
    }
}
