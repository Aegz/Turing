using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class DatabaseSymbol : Symbol
    {
        public DatabaseSymbol(SyntaxToken xoToken) : base(xoToken)
        {
        }


        public override string ToString()
        {
            return RawSQLText + 
                (Children.Count > 0 ? 
                    (String.IsNullOrEmpty(RawSQLText) ? 
                        String.Empty : 
                        ".") + base.GetChildString() : 
                    String.Empty);
        }
    }
}
