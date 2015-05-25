using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class ColumnSymbol : Symbol
    {

        public ColumnSymbol(SyntaxToken xoToken) : base (xoToken)
        {
        }

        public override string ToString()
        {
            return 
                RawSQLText + 
                (String.IsNullOrWhiteSpace(Alias) ? // If the alias is blank
                    String.Empty : // Empty text
                    " " + Alias) // Space + Alias
                    ;
        }
    }
}
