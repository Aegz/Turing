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
    public class SyntaxNode : IEnumerable<SyntaxNode>, IList<SyntaxNode>, ISyntax
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
  
        // return the token's trivia
        public List<SyntaxTrivia> LeadingTrivia
        {
            get
            {
                return Token.LeadingTrivia;
            }
            set
            {

            }
        }
        public List<SyntaxTrivia> TrailingTrivia
        {
            get
            {
                return Token.TrailingTrivia;
            }
            set
            {

            }
        }

        protected NodeStrategy Strategy { get; set; }

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
                // Exclude Skipped, but include Filler
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

        public SyntaxNode (ISyntax xoToken, int xiMaxChildCount = -1) : this (xoToken, NodeStrategyFactory.FactoryCreateStrategy(xoToken.ExpectedType), xiMaxChildCount)
        {
        }

        public SyntaxNode(ISyntax xoToken, NodeStrategy xoStrategy, int xiMaxChildCount = -1)
        {
            // Assign object vars
            iMaxChildCount = xiMaxChildCount;
            Comments = new List<StatusItem>();
            Strategy = xoStrategy;

            // Add the token according to what was given
            Type oType = xoToken.GetType();
            if (oType == typeof(SyntaxToken))
            {
                Token = (SyntaxToken)xoToken;
            }
            else if (xoToken.IsNode())
            {
                Token = ((SyntaxNode)xoToken).Token;
            }
            else
            {
                // Added trivia or something obscure
                throw new Exception("Invalid object added to SyntaxNode (" + oType.ToString() + ")");
            }
        }


        #endregion

        #region Core Consumption

        public virtual CanConsumeResult CanConsumeNode(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
        {
            return Strategy.EligibilityFn(xoContext, xbIsPreconsumption);
        }

        /// <summary>
        /// This node will try and consume items from the list
        /// </summary>
        /// <param name="xoContext"></param>
        /// <returns></returns>
        public virtual Boolean TryConsumeFromContext(ParsingContext xoContext)
        {
            // Until we break
            while (true)
            {
                // Generate a new context every time
                ParsingContext oContext = new ParsingContext(this, xoContext.List);

                // Call the Consumption fn
                CanConsumeResult eResult = Strategy.EligibilityFn(oContext, false);

                // Switch based on the result of the attempt
                switch (eResult)
                {
                    case CanConsumeResult.Consume:
                        // Create a new node
                        ISyntax oNew = Strategy.TryConsumeNextFn(oContext, false);

                        // Append the new node to the list
                        oContext.List.Insert(oNew);

                        // Consume it
                        if (AddChildAndPostProcess(oContext, false))
                        {
                            // Set the variable once
                            if (!bHasConsumedNodes)
                            {
                                bHasConsumedNodes = true;
                            }
                        }
                        else
                        {
                            // Couldn't add the node for some reason
                            this.Comments.Add(new StatusItem("Could not add:" + oNew.RawSQLText));
                        }
                        break;
                    case CanConsumeResult.Skip:
                        //xoList.PopToken(); // Skip the next node
                        break;
                    case CanConsumeResult.Complete:
                        // If post validation fails and we fixed the issue
                        if (!Strategy.ValidationFn(oContext) && TryAndResolveIssues(oContext))
                        {
                            // keep processing
                            break;
                        }

                        // Else Terminating case
                        return bHasConsumedNodes;
                    case CanConsumeResult.Unknown:
                        // Try and fix any issues
                        if (TryAndResolveIssues(oContext))
                        {
                            // Found a solution, keep processing
                            break;
                        }

                        // Else, fail and leave here
                        return bHasConsumedNodes;
                }                           
            }
        }

        private Boolean TryAndResolveIssues(ParsingContext xoContext)
        {
            // We can scan the next tokens Leading Trivia
            // And the last token's trailing trivia for a keyword
            if (ResolutionGenerator.ScanSurroundingTriviaForKeyword(xoContext))
            {
                // Found a solution, keep processing
                return true;
            }

            return false;

        }

        /// <summary>
        /// Postprocessing method that can be overriden if some activity needs to be
        /// done immediately after a node is constructed
        /// </summary>
        /// <param name="xoNode"></param>
        /// <param name="xoContext.List"></param>
        /// <returns></returns>
        private Boolean AddChildAndPostProcess(ParsingContext xoContext, Boolean xbIsPreconsumption = false)
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

        public Boolean IsNode()
        {
            return true;
        }

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
