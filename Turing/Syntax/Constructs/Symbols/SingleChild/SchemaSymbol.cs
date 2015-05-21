using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class SchemaSymbol : Symbol
    {
        public SchemaSymbol(String xsRawText) : base (xsRawText)
        {
        }


        public override string ToString()
        {
            return RawSQLText + (Children.Count > 0 ? "." + base.GetChildString() : " " + Alias);
        }
    }
}
