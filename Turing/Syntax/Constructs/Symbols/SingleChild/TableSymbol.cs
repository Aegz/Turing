using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    public class TableSymbol : Symbol
    {
        public TableSymbol(SyntaxToken xoToken, NodeStrategy xoStrategy) : base (xoToken, xoStrategy)
        {
        }
        public TableSymbol(SyntaxToken xoToken) : base (xoToken)
        {
        }

        public override string ToString()
        {
            Boolean bHasText = !String.IsNullOrEmpty(RawSQLText);
            Boolean bAliasIsValid = !String.IsNullOrEmpty(Alias);
            return RawSQLText + 
                (Children.Count > 0 ? // Has Children
                    (bHasText ? "." : "") + base.GetChildString() : // Child has its own text
                    (bAliasIsValid ? " " + Alias: ""));
        }
    }
}
