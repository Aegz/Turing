using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Parser;
using Turing.Syntax.Collections;

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
        public List<StatusItem> Comments;       // Comments/Errors specific to this node

        protected List<SyntaxKind> AcceptedTypes;

        SyntaxToken Token { get; set; }

        #endregion

        #region Node Attributes

        protected List<SyntaxNode> aoChildren;

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

        public virtual Boolean AddChild(SyntaxNode xoGiven)
        {
            // Just add it
            Children.Add(xoGiven);
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
            AcceptedTypes = new List<SyntaxKind>(); // Always initialise the list
            Comments = new List<StatusItem>();
        }


        #endregion


        public virtual Boolean TryConsumeList(SyntaxTokenList xoWindow)
        {
            Boolean bNodesConsumedSuccessfully = false;

            // Only do work if we have anything left to process
            if (!xoWindow.HasTokensLeftToProcess() ||   // We have stuff to process
                SyntaxNode.IsTerminatingNode(xoWindow.PeekToken().ExpectedType)) // We have not reached a terminator
            {
                // Nothing left to process
                return false;
            }

            // While we have nodes to process
            while (xoWindow.HasTokensLeftToProcess())
            {
                // Intermediate var
                SyntaxToken oNextToken = xoWindow.PeekToken();

                // If we have a terminating condition
                if (SyntaxNode.IsTerminatingNode(oNextToken.ExpectedType) || // Terminator Token
                    !AcceptedTypes.Contains(oNextToken.ExpectedType))        // Can't eat the next token
                {
                    // Break out and say it failed
                    return bNodesConsumedSuccessfully;
                }

                // Get the next Token
                oNextToken = xoWindow.PopToken();

                // 1. Construct a Node based off the next token
                SyntaxNode oNode = this.ConvertTokenIntoNode(oNextToken, xoWindow);

                // Add the child to this nodes children
                if (AddChild(oNode))
                {
                    if (!bNodesConsumedSuccessfully)
                    {
                        bNodesConsumedSuccessfully = true;
                    }

                    // 2. Depth first traversal from the child
                    if (oNode.TryConsumeList(xoWindow))
                    {
                        // If it successfully consumed something
                    }
                    // ?? Scan ahead using the NEXT node to see if it can consume anything?
                    // This lets us know when to move on to trying to buil

                    // 3. Return this object after it consumed all the tokens it could
                }
                else
                {
                    // Couldn't be added? Error and skip node
                    StatusItem oError = new StatusItem(
                        String.Format(
                            ErrorMessageLibrary.ADD_INVALID_NODE,
                            ReasonMessageLibrary.DUPLICATE_NODE,
                            this.RawSQLText,
                            oNextToken.RawSQLText));

                    //
                    this.Comments.Add(oError);
                }
            }
            
            // Default to true
            return bNodesConsumedSuccessfully;
        }


        public virtual SyntaxNode ConvertTokenIntoNode(SyntaxToken xoToken, SyntaxTokenList xoList)
        {
            // Always try and perform a contextual conversion
            return SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(xoToken, xoList); ;
        }


        public override String ToString()
        {
            return RawSQLText + " " + GetChildString();
        }


        public virtual String GetChildString()
        {
            return String.Join(" ", Children.Select((oNode) => oNode.ToString()));
        }


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
                xeType == SyntaxKind.OuterJoinKeyword;
        }

        #endregion
    }
}
