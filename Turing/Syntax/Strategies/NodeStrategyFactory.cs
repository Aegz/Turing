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
            NodeStrategyFactory.DefaultCanProcessNext,
            NodeStrategyFactory.DefaultConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy NULL_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.NullTwoArgument,
            NodeStrategyFactory.NullTwoArgument,
            NodeStrategyFactory.DefaultConvertToken,
            NodeStrategyFactory.NullThreeArgument); 

        //public static readonly NodeStrategy IDENTIFIER_STRATEGY = new NodeStrategy(
        //    NodeStrategyFactory.IdentifierCanConsumeNext,
        //    NodeStrategyFactory.DefaultCanProcessNext,
        //    NodeStrategyFactory.DefaultConvertToken,
        //    NodeStrategyFactory.DefaultAddChild); 

        public static readonly NodeStrategy SYMBOL_LIST_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.SymbolListCanConsumeNext,
            NodeStrategyFactory.SymbolListCanProcessNext,
            NodeStrategyFactory.ColumnSymbolConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy BINARY_EXPRESSION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.BinaryExpressionCanConsumeNext,
            NodeStrategyFactory.DefaultCanProcessNext,
            NodeStrategyFactory.ColumnSymbolConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy UNARY_EXPRESSION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.ExpressionCanConsumeNext,
            NodeStrategyFactory.UnaryExpressionProcessNext,
            NodeStrategyFactory.ColumnSymbolConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy JOIN_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.JoinCanConsumeNext,
            NodeStrategyFactory.DefaultCanProcessNext,
            NodeStrategyFactory.TableSymbolConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy SUBQUERY_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.SubQueryCanConsumeNext,
            NodeStrategyFactory.SubQueryProcessNext,
            NodeStrategyFactory.DefaultConvertToken,
            NodeStrategyFactory.DefaultAddChild); // Default

        public static readonly NodeStrategy FUNCTION_STRATEGY = new NodeStrategy(
            NodeStrategyFactory.FunctionCanConsumeNext,
            NodeStrategyFactory.FunctionCanProcessNext,
            NodeStrategyFactory.ColumnSymbolConvertToken,
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
                    NodeStrategyFactory.DefaultCanConsumeNext,
                    NodeStrategyFactory.DefaultCanProcessNext,
                    NodeStrategyFactory.DefaultConvertToken,
                    NodeStrategyFactory.DefaultAddChild); // Default

                // Create the strategy
                switch (xeKind)
                {
                    case SyntaxKind.SelectKeyword:
                        oReturnNode.ConsumptionFn = SelectCanConsumeNext;
                        oReturnNode.ConvertTokenFn = SelectConvertToken;
                        break;
                    case SyntaxKind.FromKeyword:
                        oReturnNode.ConsumptionFn = FromCanConsumeNext;
                        oReturnNode.ConvertTokenFn = TableSymbolConvertToken;
                        break;
                    case SyntaxKind.WhereKeyword:
                    case SyntaxKind.OnKeyword:
                        oReturnNode.ConsumptionFn = ExpressionCanConsumeNext;
                        //oReturnNode.ConvertTokenFn = ColumnSymbolConvertToken;
                        break;
                    case SyntaxKind.IdentifierToken:
                        oReturnNode.ConsumptionFn = IdentifierCanConsumeNext;
                        break;

                    // Open Parenthesis must have the ability to close itself
                    case SyntaxKind.OpenParenthesisToken:
                        oReturnNode.ConsumptionFn = UnaryExpressionProcessNext;
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
            return DefaultConvertToken(xoCurrentNode, xoList);
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
                return DefaultConvertToken(xoCurrentNode, xoList);
            }
        }

        /// <summary>
        /// Used by WHERE and ON
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean ExpressionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eKind = xoList.PeekToken().ExpectedType;

            return
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsArithmaticOperator(eKind);
        }

        #endregion

        #region Select

        public static SyntaxNode SelectConvertToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Build a Symbol Composite
            SyntaxToken oCurrentToken = xoList.PeekToken();

            // If we need to perform a context sensitive conversion
            if (SyntaxKindFacts.IsIdentifier(oCurrentToken.ExpectedType) ||
                oCurrentToken.ExpectedType == SyntaxKind.StarToken) // * in Column is allowed
            {
                // Initialise a list
                SymbolList oList = new SymbolList();

                // Add this column
                //oList.AddChild(ColumnSymbolConvertToken(xoCurrentNode, xoList));

                // generate a symbol list (which will consume anything else that is a column)
                return oList;
            }

            // Default to using the original conversion
            return DefaultConvertToken(xoCurrentNode, xoList);
        }

        public static Boolean SelectCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Intermediate var
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            // Try convert
            return SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken ||
                ((int)oKind >= (int)SyntaxKind.FromKeyword &&
                (int)oKind <= (int)SyntaxKind.HavingKeyword);
        }

        #endregion

        #region FROM

        public static Boolean FromCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;
            return 
                SyntaxKindFacts.IsIdentifier(oKind) ||
                SyntaxKindFacts.IsJoinKeyword(oKind);
        }

        #endregion

        #region JOIN/ON

        public static Boolean JoinCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;
            return
                SyntaxKindFacts.IsIdentifier(oKind) ||
                oKind == SyntaxKind.OnKeyword;
        }

        #endregion

        #region SubQuery

        public static Boolean SubQueryCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;
            return
                oKind == SyntaxKind.SelectKeyword;
        }

        public static Boolean SubQueryProcessNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Use the generic Unary Expression function
            if (UnaryExpressionProcessNext(xoCurrentNode, xoList))
            {
                // Just add an Alias
                ((Symbol)xoCurrentNode).Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                return true;
            }

            return DefaultCanProcessNext(xoCurrentNode, xoList);
        }

        #endregion

        #region Functions


        public static Boolean FunctionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Intermediate var
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            // Depending on what Function we have, 
            // we can accept different keywords

            // Try convert
            return SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken;
        }

        /// <summary>
        /// Finish up as soon as we come across a Close Parenthesis
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean FunctionCanProcessNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we get a comma, just drop it
            if (xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                xoCurrentNode.IsComplete = true;

                // Scan ahead for Alias
                return true;
            }

            return DefaultCanProcessNext(xoCurrentNode, xoList);
        }

        #endregion

        #region Expressions

        public static Boolean BinaryExpressionCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            SyntaxKind eKind = xoList.PeekToken().ExpectedType;

            return
                SyntaxKindFacts.IsIdentifierOrExpression(eKind) || // Identifiers and Expressions are allowed here
                //SyntaxKindFacts.IsAdjunctConditionalOperator(eKind) || // AND OR
                SyntaxKindFacts.IsConditionalOperator(eKind) || // = >= IN
                SyntaxKindFacts.IsUnaryOperator(eKind) || // NOT
                SyntaxKindFacts.IsArithmaticOperator(eKind);
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
                return DefaultConvertToken(xoCurrentNode, xoList);
            }
        }

        public static Boolean UnaryExpressionProcessNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                xoCurrentNode.IsComplete = true;
                return true;
            }

            return DefaultCanProcessNext(xoCurrentNode, xoList);
        }

        #endregion

        #region SymbolList

        public static Boolean SymbolListCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Intermediate var
            SyntaxKind oKind = xoList.PeekToken().ExpectedType;

            // Try convert
            return SyntaxKindFacts.IsIdentifierOrExpression(oKind) ||
                oKind == SyntaxKind.StarToken;
        }

        /// <summary>
        /// Finish up as soon as we come across a Close Parenthesis
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean SymbolListCanProcessNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (xoCurrentNode.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
            {
                xoList.PopToken();
                xoCurrentNode.IsComplete = true;
                return true;
            }
            // If we get a comma, just drop it
            else if (xoList.PeekToken().ExpectedType == SyntaxKind.CommaToken)
            {
                xoList.PopToken();
                return true;
            }

            return DefaultCanProcessNext(xoCurrentNode, xoList);
        }
        
        #endregion

        #region Identifier

        public static Boolean IdentifierCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            return false;
        }

        #endregion

        #region Default Methods

        /// <summary>
        /// Returns true if the next node is something this node can interpret
        /// and returns false if it cannot do anything with the next node
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean DefaultCanConsumeNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Can we eat the next node?
            return !SyntaxKindFacts.IsTerminatingNode(xoList.PeekToken().ExpectedType); // Terminator Token
        }

        /// <summary>
        /// Used when you can simply process a node but not construct anything from it
        /// if it succeeds then it has successfully processed the node
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static Boolean DefaultCanProcessNext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Can we preprocess the next node
            return false;
        }

        /// <summary>
        /// Provides context sensitive generation of a new Node
        /// </summary>
        /// <param name="xoCurrentNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public static SyntaxNode DefaultConvertToken(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            // Always try and perform a contextual conversion
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoCurrentNode, xoList); ;
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
                //
                xoCurrentNode.InsertStatusMessage(String.Format(
                        ErrorMessageLibrary.ADD_INVALID_NODE,
                        ReasonMessageLibrary.DUPLICATE_NODE,
                        xoCurrentNode.RawSQLText,
                        xoNode.RawSQLText));

                return false;
            }
        }

        #endregion

        #region Null Methods

        public static Boolean NullTwoArgument(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            return false;
        }

        public static Boolean NullThreeArgument(SyntaxNode xoCurrentNode, SyntaxNode xoNewNode, SyntaxTokenList xoList)
        {
            return false;
        }


        #endregion
        
        #endregion Node Methods
    }
}
