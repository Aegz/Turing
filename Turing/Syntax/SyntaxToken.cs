using System;
using System.Collections.Generic;
using Turing.Diagnostics;

namespace Turing.Syntax
{
    /// <summary>
    /// Core Token object. Does not inherit from anything but is a component
    /// of the Syntax Node
    /// </summary>
    public class SyntaxToken : SyntaxNode
    {

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

        public SyntaxToken(SyntaxKind xeType, List<SyntaxTrivia> xoLeading, List<SyntaxTrivia> xoTrailing, String xsRawText) : base(xeType, xsRawText)
        {
        }

        public SyntaxToken(SyntaxKind xeType, String xsRawText) : this (xeType, null, null, xsRawText)
        {
        }

    }
}