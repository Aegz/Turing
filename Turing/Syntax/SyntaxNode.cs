using System;
using System.Collections;
using System.Collections.Generic;
using Turing.Diagnostics;
using Turing.Parser;
using System.Linq;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Exceptions;

namespace Turing.Syntax
{
    public class SyntaxNode : IEnumerable<SyntaxNode>, IList<SyntaxNode>
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

        public SyntaxToken Token { get; set; }

        protected NodeStrategy oStrategy; 

        //
        protected Boolean bHasConsumedNodes = false; // Tells you when it has actually built anything
        protected int iMaxChildCount = -1;

        #endregion

        #region List Functions

        protected List<SyntaxNode> aoChildren; // Currently at protected for Query and Statement

        protected List<SyntaxNode> Children
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

        public SyntaxNode this[int xiIndex]
        {
            get
            {
                return Children[xiIndex];
            }
        }

        public Boolean Exists (Predicate<SyntaxNode> xoFunc)
        {
            return Children.Exists(xoFunc);
        }

        public void RemoveAt(int xiIndex)
        {
            Children.RemoveAt(xiIndex);
        }

        public virtual int Count
        {
            get
            {
                // ?? TODO: Exclude Exception Nodes
                return Children.Where((oNode) => oNode.GetType() != typeof(SkippedNode)).Count();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<SyntaxNode>)Children).IsReadOnly;
            }
        }

        SyntaxNode IList<SyntaxNode>.this[int index]
        {
            get
            {
                return ((IList<SyntaxNode>)Children)[index];
            }

            set
            {
                ((IList<SyntaxNode>)Children)[index] = value;
            }
        }

        public virtual Boolean Add(SyntaxNode xoGiven)
        {
            // Dont add if we are full up
            if (IsFull())
            {
                return false;
            }

            // Add it
            Children.Add(xoGiven);

            // Set the parent
            xoGiven.Parent = this;

            // Default to true
            return true;
        }

        public virtual Boolean IsFull()
        {
            return Children.Count == iMaxChildCount;
        }

        #endregion

        #region Construction

        public SyntaxNode (SyntaxToken xoToken, int xiMaxChildCount = -1) : this (xoToken, NodeStrategyFactory.FactoryCreateStrategy(xoToken.ExpectedType), xiMaxChildCount)
        {
        }

        public SyntaxNode(SyntaxToken xoToken, NodeStrategy xoStrategy, int xiMaxChildCount = -1)
        {
            iMaxChildCount = xiMaxChildCount;
            Token = xoToken;
            Comments = new List<StatusItem>();
            oStrategy = xoStrategy;
        }


        #endregion

        #region Core Consumption

        public virtual CanConsumeResult CanConsumeNode(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return oStrategy.EligibilityFn(xoContext, xbIsPreconsumption);
        }

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
                // Generate a new context every time
                ParsingContext oContext = new ParsingContext(this, null, xoList);

                // Call the Consumption fn
                CanConsumeResult eResult = oStrategy.EligibilityFn(oContext, false);

                // Switch based on the result of the attempt
                switch (eResult)
                {
                    case CanConsumeResult.Consume:
                        oContext.NewNode = oStrategy.TryConsumeNextFn(oContext, false);
                        if (oStrategy.PostProcessFn(oContext, false))
                        {
                            // Set the variable once
                            if (!bHasConsumedNodes)
                            {
                                bHasConsumedNodes = true;
                            }
                        }
                        else
                        {
                            // When we fail, we need to be able to fix this
                            this.Comments.Add(new StatusItem("Could not generate:" + oContext.NewNode.RawSQLText));
                        }
                        break;
                    case CanConsumeResult.Skip:
                        //xoList.PopToken(); // Skip the next node
                        break;
                    case CanConsumeResult.Complete:
                        return bHasConsumedNodes;
                    case CanConsumeResult.Unknown:
                        // We can scan the next tokens Leading Trivia
                        // And the last token's trailing trivia for a keyword
                        if (ResolutionGenerator.ScanSurroundingTriviaForKeyword(oContext))
                        {
                            // Found a solution, keep processing
                            break;
                        }


                        return bHasConsumedNodes;
                }                           
            }
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
                        Add(oLoopingVar);

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

        public SyntaxNode GetLastChild()
        {
            if (Children.Count == 0)
            {
                return this;
            }
            else
            {
                return Children[Count - 1].GetLastChild();
            }
        }

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

        public SyntaxNode FindFirstParent(Func<SyntaxNode, Boolean> xeFunction)
        {
            if (Parent != null)
            {
                if (xeFunction(Parent))
                {
                    return Parent;
                }
                else
                {
                    return Parent.FindFirstParent(xeFunction);
                }
            }
            else
            {
                return null;
            }

        }

        public IEnumerator<SyntaxNode> GetEnumerator()
        {
            return ((IEnumerable<SyntaxNode>)Children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<SyntaxNode>)Children).GetEnumerator();
        }

        public int IndexOf(SyntaxNode item)
        {
            return ((IList<SyntaxNode>)Children).IndexOf(item);
        }

        public void Insert(int index, SyntaxNode item)
        {
            ((IList<SyntaxNode>)Children).Insert(index, item);
        }

        void ICollection<SyntaxNode>.Add(SyntaxNode item)
        {
            ((IList<SyntaxNode>)Children).Add(item);
        }

        public void Clear()
        {
            ((IList<SyntaxNode>)Children).Clear();
        }

        public bool Contains(SyntaxNode item)
        {
            return ((IList<SyntaxNode>)Children).Contains(item);
        }

        public void CopyTo(SyntaxNode[] array, int arrayIndex)
        {
            ((IList<SyntaxNode>)Children).CopyTo(array, arrayIndex);
        }

        public bool Remove(SyntaxNode item)
        {
            return ((IList<SyntaxNode>)Children).Remove(item);
        }

        #endregion

    }
}
