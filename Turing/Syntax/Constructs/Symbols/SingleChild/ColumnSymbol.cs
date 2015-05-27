using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class ColumnSymbol : Symbol
    {
        public ColumnSymbol(SyntaxToken xoToken, NodeStrategy xoStrategy) : base (xoToken, xoStrategy)
        {

        }

        public ColumnSymbol(SyntaxToken xoToken) : base (xoToken)
        {
        }

        public override string ToString()
        {
            return 
                RawSQLText + GetChildString() + 
                (String.IsNullOrWhiteSpace(Alias) ? // If the alias is blank
                    String.Empty : // Empty text
                    " " + Alias) // Space + Alias
                    ;
        }
    }
}
