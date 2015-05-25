using System;
using System.Collections.Generic;
using System.Linq;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Expressions;
using Turing.Syntax.Constructs.Keywords;
using Turing.Syntax.Constructs.Symbols.Collections;

namespace Turing.Syntax
{
    public abstract class SyntaxNode 
    {
        #region Object Attributes

        public String RawSQLText // Always store the raw SQL text if you can for reproduction
        {
            get { return Token.RawSQLText; }
            set { Token.RawSQLText = value; }
        }

        public SyntaxKind ExpectedType // Always store the raw SQL text if you can for reproduction
        {
            get { return Token.ExpectedType; }
            set { Token.ExpectedType = value; }
        }

        public SyntaxNode Parent { get; set; }  // Parent Node
        private List<StatusItem> Comments;       // Comments/Errors specific to this node

        protected List<SyntaxKind> ConsumableTypes;
        protected List<SyntaxKind> ClosingTypes; // used to represent the types that close this node off

        protected SyntaxToken Token { get; set; }


        //
        protected Boolean bHasConsumedNodes = false; // Tells you when it has actually built anything

        #endregion

        #region Node Attributes

        protected List<SyntaxNode> aoChildren; // Currently at protected for Query and Statement

        public List<SyntaxNode> Children
        {
            get
            {
                if (aoChildren == null)
                {
                    aoChildren = new List<SyntaxNode>();
                }

                return aoChildren;
            }
        }

        public virtual Boolean AddChild(SyntaxNode xoGiven)
        {
            // Just add it
            Children.Add(xoGiven);

            // Set the parent
            xoGiven.Parent = this;

            // Default to true
            return true;
        }

        #endregion

        #region Construction

        public SyntaxNode() : this (new SyntaxToken(SyntaxKind.UnknownToken, String.Empty))
        {
        }

        public SyntaxNode(SyntaxToken xoToken)
        {
            Token = xoToken;
            ConsumableTypes = new List<SyntaxKind>(); // Always initialise the list
            ClosingTypes = new List<SyntaxKind>();
            Comments = new List<StatusItem>();
        }


        #endregion

        /// <summary>
        /// Returns true if the next node is something this node can interpret
        /// and returns false if it cannot do anything with the next node
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public Boolean CanConsumeNextNode(SyntaxTokenList xoList)
        {
            if (ClosingTypes.Contains(xoList.PeekToken().ExpectedType))
            {
                // Kill off the token and return false
                xoList.PopToken();
                return false;
            }
            else
            {
                return
                    !SyntaxNode.IsTerminatingNode(xoList.PeekToken().ExpectedType) && // Terminator Token
                    ConsumableTypes.Contains(xoList.PeekToken().ExpectedType);        // Can't eat the next token
            }
        }

        /// <summary>
        /// Used when you can simply process a node but not construct anything from it
        /// if it succeeds then it has successfully processed the node
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public virtual Boolean CanProcessNextNode(SyntaxTokenList xoList)
        {
            return false;
        }

        /// <summary>
        /// Allows this node to consume tokens from the given window
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public virtual Boolean TryConsumeList(SyntaxTokenList xoList)
        {
            // Only do work if we have anything left to process
            if (!xoList.HasTokensLeftToProcess() ||   // We have stuff to process
                SyntaxNode.IsTerminatingNode(xoList.PeekToken().ExpectedType)) // We have not reached a terminator
            {
                // Nothing left to process
                return false;
            }

            // While we have nodes to process
            while (xoList.HasTokensLeftToProcess())
            {              
                // PRE
                if (CanProcessNextNode(xoList))
                {
                    // Do any necessary pre processing which can include things 
                    // such as cleansing and dropping irrelevant nodes
                    // Cleanse the next node if necessary

                }
                // CONSTRUCT
                else if (CanConsumeNextNode(xoList))
                {
                    // 1. Construct a Node based off the next token
                    SyntaxNode oNode = this.ConvertTokenIntoNode(xoList);

                // POST
                    // Do any necessary post processing on the given node
                    // Which includes adding it as a child
                    if (PostprocessNodeAndAddAsChild(oNode, xoList))
                    {
                        // Set the variable once
                        if (!bHasConsumedNodes)
                        {
                            bHasConsumedNodes = true;
                        }
                    }

                }
                // TERMINATE
                else
                {
                    return bHasConsumedNodes;
                }
            }
            
            // Default to true
            return bHasConsumedNodes;
        }

        /// <summary>
        /// Allows this node to determine the order and type of nodes it creates
        /// </summary>
        /// <param name="xoToken"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public virtual SyntaxNode ConvertTokenIntoNode(SyntaxTokenList xoList)
        {
            // Always try and perform a contextual conversion
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoList); ;
        }

        /// <summary>
        /// Postprocessing method that can be overriden if some activity needs to be
        /// done immediately after a node is constructed
        /// </summary>
        /// <param name="xoNode"></param>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public virtual Boolean PostprocessNodeAndAddAsChild(SyntaxNode xoNode, SyntaxTokenList xoList)
        {
            // Add the child to this nodes children
            if (AddChild(xoNode))
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
                // Couldn't be added? Error and skip node
                StatusItem oError = new StatusItem(
                    String.Format(
                        ErrorMessageLibrary.ADD_INVALID_NODE,
                        ReasonMessageLibrary.DUPLICATE_NODE,
                        this.RawSQLText,
                        xoNode.RawSQLText));

                //
                this.Comments.Add(oError);
                return false;
            }
        }


        #region Consume Prev Sibling

        // Delegate function responsible for determining if a sibling
        // is eligible for consumption
        //public delegate Boolean IsPreviousNodeEligible(SyntaxNode xoNode);

        protected virtual Boolean PreviousChildIsEligible(SyntaxNode xoNode)
        {
            return false;
        }

        /// <summary>
        /// Primarily used for compound styled nodes which need to consume the
        /// previous sibling for left associativity
        /// </summary>
        /// <returns></returns>
        public virtual Boolean TryConsumePreviousSibling()
        {
            // Check if we can even consume the previous sibling
            if (Parent.Children.Count >= 0)
            {
                // Work backwards until we find a suitable candidate
                for (int iIndex = Parent.Children.Count - 1; iIndex >= 0; iIndex--)
                {
                    SyntaxNode oLoopingVar = Parent.Children[iIndex];

                    // If we find a suitable candidate for consumption
                    if (PreviousChildIsEligible(oLoopingVar) && !this.Equals(oLoopingVar))
                    {
                        // Consume the previous sibling
                        AddChild(oLoopingVar);

                        // Remove the sibling from the parent
                        Parent.Children.RemoveAt(iIndex);

                        // Return true here
                        return true;
                    }
                }
            }

            // Default to false
            return false;
        }

        #endregion

        #region Common Utility

        public override String ToString()
        {
            return RawSQLText + " " + GetChildString();
        }

        public virtual String GetChildString()
        {
            return String.Join(" ", Children);
        }

        public SyntaxNode FindFirst(SyntaxKind xeExpectedKind)
        {
            if (Token.ExpectedType == xeExpectedKind)
            {
                return this;
            }
            else
            {
                foreach (SyntaxNode oChildren in Children)
                {
                    SyntaxNode oChildFound = oChildren.FindFirst(xeExpectedKind);
                    if (oChildFound != null)
                    {
                        return oChildFound;
                    }
                }
            }
            return null;
        }

        public virtual void InsertStatusMessage(String xsMessage)
        {
            Comments.Add(new StatusItem(xsMessage));
        }

        #endregion

        #region Static Identification Functions

        public static Boolean IsIdentifier(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.IdentifierToken;
        }

        public static Boolean IsCoreNode(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.SelectKeyword ||
                xeKind == SyntaxKind.InsertKeyword ||
                xeKind == SyntaxKind.UpdateKeyword ||
                xeKind == SyntaxKind.CreateKeyword ||
                xeKind == SyntaxKind.AlterKeyword ||
                xeKind == SyntaxKind.WithKeyword ||
                xeKind == SyntaxKind.DeleteKeyword;
        }

        public static Boolean IsTerminatingNode(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.EOFToken ||
                xeKind == SyntaxKind.EOFTrivia ||
                xeKind == SyntaxKind.SemiColonToken;
        }

        public static Boolean IsOperator(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.GreaterThanOrEqualToken ||
                xeKind == SyntaxKind.GreaterThanToken ||
                xeKind == SyntaxKind.LessThanOrEqualToToken ||
                xeKind == SyntaxKind.LessThanToken ||
                xeKind == SyntaxKind.DiamondToken ||

                xeKind == SyntaxKind.NotKeyword ||
                xeKind == SyntaxKind.IsKeyword ||
                xeKind == SyntaxKind.InKeyword ||

                xeKind == SyntaxKind.EqualsToken ||
                xeKind == SyntaxKind.PlusToken ||
                xeKind == SyntaxKind.MinusToken ||
                xeKind == SyntaxKind.SlashToken ||
                xeKind == SyntaxKind.StarToken ||
                xeKind == SyntaxKind.PlusToken;
        }

        public static Boolean IsJoinTypeKeyword(SyntaxKind xeType)
        {
            return
                xeType == SyntaxKind.InnerJoinKeyword ||
                xeType == SyntaxKind.LeftJoinKeyword ||
                xeType == SyntaxKind.RightJoinKeyword ||
                xeType == SyntaxKind.CrossJoinKeyword ||
                xeType == SyntaxKind.OuterKeyword;
        }

        #endregion
    }
}
