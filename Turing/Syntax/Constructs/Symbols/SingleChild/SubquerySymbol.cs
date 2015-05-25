using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class SubquerySymbol : TableSymbol
    {
        public SubquerySymbol(SyntaxToken xoToken) : base (xoToken)
        {
        }

        public override string ToString()
        {
            return "(" + (Children.Count > 0 ? base.GetChildString() : " ") + ")" + Alias;
        }
    }
}
