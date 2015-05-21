
using System;
using System.Collections;
using System.Collections.Generic;
using Turing.Syntax.Constructs;

namespace Turing.Syntax.Collections
{
    public class SyntaxTokenList 
    {
        List<SyntaxToken> aoItems;

        public SyntaxTokenList()
        {
            aoItems = new List<SyntaxToken>();
            Position = 0;
        }

        public void Add(SyntaxToken xoToken)
        {
            if (xoToken != null)
            {
                aoItems.Add(xoToken);
            }
        }

        public int Count
        {
            get
            {
                return aoItems.Count;
            }
        }

        public int Position { get; set; }

        public Boolean HasTokensLeftToProcess()
        {
            return Position < Count;
        }

        public SyntaxToken this[int xiIndex]
        {
            get
            {
                return aoItems[xiIndex];
            }
        }

        public SyntaxToken PeekToken()
        {
            if (Position < Count)
            {
                return this[Position];
            }
            else
            {
                return new ExceptionSyntaxToken();
            }
        }

        public SyntaxToken PeekToken(int xiExtra)
        {
            if (Position + xiExtra < Count)
            {
                return this[Position + xiExtra];
            }
            else
            {
                return new ExceptionSyntaxToken();
            }
        }

        public SyntaxToken PopToken()
        {
            SyntaxToken oTemp = PeekToken();
            Position++;
            return oTemp;
        }

        public List<SyntaxToken> PopTokens(int xiLength)
        {
            List<SyntaxToken> aoReturn = new List<SyntaxToken>();

            for (int iIndex = 0; iIndex < xiLength && iIndex < Count; iIndex++)
            {
                aoReturn.Add(PopToken());
            }

            return aoReturn;
        }
    }
}
