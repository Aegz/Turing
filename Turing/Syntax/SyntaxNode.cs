﻿using System;
using System.Collections.Generic;
using Turing.Diagnostics;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Syntax
{
    public class SyntaxNode 
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
        public List<StatusItem> Comments;       // Comments/Errors specific to this node

        protected SyntaxToken Token { get; set; }

        protected NodeStrategy oStrategy;

        //
        protected Boolean bHasConsumedNodes = false; // Tells you when it has actually built anything
        protected int iMaxChildCount = -1;

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
            // If there is a restriction
            if (iMaxChildCount != -1)
            {
                // Check how many children we have
                if (Children.Count >= iMaxChildCount)
                {
                    // Do not add it
                    return false;
                }
            }

            // Add it
            Children.Add(xoGiven);

            // Set the parent
            xoGiven.Parent = this;

            // Default to true
            return true;
        }

        #endregion

        #region Construction

        public SyntaxNode (SyntaxToken xoToken, int xiMaxChildCount = -1) : this (xoToken, NodeStrategyFactory.DEFAULT_STRATEGY, xiMaxChildCount)
        {
        }

        public SyntaxNode(SyntaxToken xoToken, NodeStrategy xoStrategy, int xiMaxChildCount = -1)
        {
            Token = xoToken;
            Comments = new List<StatusItem>();

            oStrategy = xoStrategy;
        }


        #endregion

        #region Core Consumption

        /// <summary>
        /// Allows this node to consume tokens from the given window
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public virtual Boolean TryConsumeList(SyntaxTokenList xoList)
        {
            // Until we break
            while (true)
            {
                // Call the Consumption fn
                CanConsumeResult oResult = oStrategy.EligibilityFn(this, xoList);

                // Switch based on the result of the attempt
                switch (oResult)
                {
                    case CanConsumeResult.Consume:
                        SyntaxNode oNewNode = oStrategy.TryConsumeNextFn(this, xoList);
                        if (oStrategy.PostProcessFn(this, oNewNode, xoList))
                        {
                            // Set the variable once
                            if (!bHasConsumedNodes)
                            {
                                bHasConsumedNodes = true;
                            }
                        }
                        break;
                    case CanConsumeResult.Skip:
                        //xoList.PopToken(); // Skip the next node
                        break;
                    case CanConsumeResult.Complete:
                        return bHasConsumedNodes;
                }                    
                
            }

            // Default to true
            return bHasConsumedNodes;
        }

        #endregion


        #region Consume Prev Sibling

        /// <summary>
        /// Primarily used for compound styled nodes which need to consume the
        /// previous sibling for left associativity
        /// </summary>
        /// <returns></returns>
        public virtual Boolean TryConsumePreviousSibling(Func<SyntaxKind, Boolean> xoEligibilityCriteria)
        {
            // Check if we can even consume the previous sibling
            if (Parent != null && Parent.Children.Count >= 0)
            {
                // Work backwards until we find a suitable candidate
                for (int iIndex = Parent.Children.Count - 1; iIndex >= 0; iIndex--)
                {
                    SyntaxNode oLoopingVar = Parent.Children[iIndex];

                    // If we find a suitable candidate for consumption
                    if (xoEligibilityCriteria(oLoopingVar.ExpectedType) && 
                        !this.Equals(oLoopingVar))
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

        #endregion

    }
}
