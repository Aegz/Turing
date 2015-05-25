using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class SchemaSymbol : Symbol
    {
        public SchemaSymbol(SyntaxToken xoToken) : base (xoToken)
        {

        }

        public override string ToString()
        {
            return RawSQLText +
                (Children.Count > 0 ?
                    "." + base.GetChildString() :
                    String.Empty);
        }
    }
}
