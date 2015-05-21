using System;
using System.Collections.Generic;
using Turing.Diagnostics;
using Turing.Syntax.Collections;

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

        public override bool AddChild(SyntaxNode xoGiven)
        {
            // Tokens do not accept children
            return false;
        }

        public override bool TryConsumeList(SyntaxTokenList xoWindow)
        {
            // Tokens do not consume
            return false;
        }

    }
}