
using System.Collections;
using System.Collections.Generic;

namespace Turing.Syntax.Collections
{
    public class SyntaxTokenList : IEnumerable<SyntaxToken> 
    {
        List<SyntaxToken> aoItems;

        public SyntaxTokenList()
        {
            aoItems = new List<SyntaxToken>();
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

        public SyntaxToken this[int xiIndex]
        {
            get
            {
                return aoItems[xiIndex];
            }
        }

        public IEnumerator<SyntaxToken> GetEnumerator()
        {
            return ((IEnumerable<SyntaxToken>)aoItems).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<SyntaxToken>)aoItems).GetEnumerator();
        }
    }
}
