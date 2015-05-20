using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Syntax.Collections;

namespace Turing.Syntax
{
    public class SyntaxNode 
    {
        #region Object Attributes

        public String RawSQLText { get; set; }  // Always store the raw SQL text if you can for reproduction
        public SyntaxNode Parent { get; set; }  // Parent Node
        public List<StatusItem> Comments;       // Comments/Errors specific to this node
        public SyntaxKind ExpectedType { get; set; } // The Expected type of this node

        #endregion

        #region Node Attributes

        protected List<SyntaxNode> aoChildren;

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

        #endregion

        #region Construction
        /// <summary>
        /// SyntaxNodes can hold a SyntaxToken (which will be supplementary
        /// to the underlying tree)
        /// </summary>
        /// <param name="xoToken"></param>
        public SyntaxNode(SyntaxKind xeType, String xsRawText)
        {
            ExpectedType = xeType;
            RawSQLText = xsRawText;
            Comments = new List<StatusItem>();
        }

        public SyntaxNode() : this (SyntaxKind.UnknownToken, String.Empty)
        {
        }
        #endregion

        public virtual Boolean TryConsumeList(SyntaxTokenList xoWindow)
        {
            // Syntax Nodes themselves cannot consume anything.
            return false;
        }
    }
}
