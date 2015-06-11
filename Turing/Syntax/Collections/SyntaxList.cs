
using System;
using System.Collections.Generic;

namespace Turing.Syntax.Collections
{
    public class SyntaxList 
    {
        List<ISyntax> aoItems;

        public SyntaxList(ISyntax xoGiven)
        {
            aoItems = new List<ISyntax>()
            {
                xoGiven
            };

            Position = 0;
        }

        public SyntaxList()
        {
            aoItems = new List<ISyntax>();
            Position = 0;
        }

        public void Add(ISyntax xoToken)
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

        public ISyntax this[int xiIndex]
        {
            get
            {
                return aoItems[xiIndex];
            }
        }

        public ISyntax Peek()
        {
            if (Position < Count)
            {
                return this[Position];
            }
            else
            {
                return new SyntaxToken(SyntaxKind.EOFNode, String.Empty);
            }
        }

        public ISyntax Peek(int xiExtra)
        {
            if (Position + xiExtra < Count)
            {
                return this[Position + xiExtra];
            }
            else
            {
                return new SyntaxToken(SyntaxKind.EOFNode, String.Empty); ;
            }
        }

        public ISyntax Pop()
        {
            ISyntax oTemp = Peek();
            // Completely remove nodes that were added
            if (!oTemp.IsNode())
            {
                Position++;
                return oTemp;
            }
            else
            {
                aoItems.Remove(oTemp);
                return oTemp;
            }

        }

        public void Insert(ISyntax xoNewNode, int xiIndex = -1)
        {
            if (xiIndex == -1)
            {
                aoItems.Insert(xiIndex == -1 ? Position : xiIndex, xoNewNode);
            }
        }

        public void Insert(List<ISyntax> xaoList, int xiIndex)
        {
            aoItems.InsertRange(xiIndex, xaoList);
        }

        public List<ISyntax> Pop(int xiLength)
        {
            List<ISyntax> aoReturn = new List<ISyntax>();

            for (int iIndex = 0; iIndex < xiLength && iIndex < Count; iIndex++)
            {
                aoReturn.Add(Pop());
            }

            return aoReturn;
        }

        public ISyntax FirstOrNull(Func<SyntaxKind, Boolean> xfCriteria)
        {
            for (int iIndex = 0; iIndex + Position < Count; iIndex++)
            {
                if (xfCriteria(this[iIndex + Position].ExpectedType))
                {
                    return this[iIndex + Position];
                }
            }

            return null;
        }
    }
}
