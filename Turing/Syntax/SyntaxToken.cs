using System;
using System.Collections.Generic;
using Turing.Diagnostics;

namespace Turing.Syntax
{
    /// <summary>
    /// Core Token object. Does not inherit from anything but is a component
    /// of the Syntax Node
    /// </summary>
    public class SyntaxToken 
    {
        #region Object Attributes

        public String RawSQLText { get; set; }  // Always store the raw SQL text if you can for reproduction
        public SyntaxNode Parent { get; set; }  // Parent Node
        public List<StatusItem> Comments;       // Comments/Errors specific to this node
        public SyntaxKind ExpectedType { get; set; } // The Expected type of this node
        
        #endregion

        #region Trivia
        private List<SyntaxTrivia> aoLeadingTrivia;
        public List<SyntaxTrivia> LeadingTrivia
        {
            get
            {
                if (aoLeadingTrivia == null)
                {
                    aoLeadingTrivia = new List<SyntaxTrivia>();
                }
                return aoLeadingTrivia;
            }
            set
            {
                aoLeadingTrivia = value;
            }
        }

        private List<SyntaxTrivia> aoTrailingTrivia;
        public List<SyntaxTrivia> TrailingTrivia
        {
            get
            {
                if (aoTrailingTrivia == null)
                {
                    aoTrailingTrivia = new List<SyntaxTrivia>();
                }
                return aoTrailingTrivia;
            }
            set
            {
                aoTrailingTrivia = value;
            }
        }
        #endregion

        public SyntaxToken(SyntaxKind xeType, List<SyntaxTrivia> xoLeading, List<SyntaxTrivia> xoTrailing, String xsRawText)
        {
            ExpectedType = xeType;
            RawSQLText = xsRawText;
            Comments = new List<StatusItem>();
        }

        public SyntaxToken(SyntaxKind xeType, String xsRawText) : this (xeType, null, null, xsRawText)
        {
        }

    }
}