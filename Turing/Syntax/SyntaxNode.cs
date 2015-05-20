using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;


namespace Turing.Syntax
{
    public class SyntaxNode 
    {
        #region Object Attributes

        public SyntaxToken Token;

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

        /// <summary>
        /// Override this method to determine whether a node can be added or not
        /// </summary>
        /// <param name="xoNode"></param>
        public virtual Boolean TryAddChild(SyntaxNode xoNode)
        {
            // Just add it
            Children.Add(xoNode);

            // Return true
            return true;
        }

        #endregion

        /// <summary>
        /// SyntaxNodes can hold a SyntaxToken (which will be supplementary
        /// to the underlying tree)
        /// </summary>
        /// <param name="xoToken"></param>
        public SyntaxNode(SyntaxToken xoToken)
        {
            Token = xoToken;
        }

        /// <summary>
        /// Allow empty construction
        /// </summary>
        public SyntaxNode()
        {
            Token = new SyntaxToken(SyntaxKind.UnknownToken, "");
        }


    }
}
