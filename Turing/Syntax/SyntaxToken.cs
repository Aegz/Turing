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
    public class SyntaxToken : ISyntax
    {
        public static readonly SyntaxToken NULL_TOKEN = new SyntaxToken(SyntaxKind.NullNode, "");

        public String RawSQLText // Always store the raw SQL text if you can for reproduction
        {
            get;
            set;
        }

        public SyntaxKind ExpectedType { get; set; }

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
            this.ExpectedType = xeType;
            RawSQLText = xsRawText;
        }

        public SyntaxToken(SyntaxKind xeType, String xsRawText) : this (xeType, null, null, xsRawText)
        {
        }

        public Boolean IsNode()
        {
            return false;
        }

    }
}