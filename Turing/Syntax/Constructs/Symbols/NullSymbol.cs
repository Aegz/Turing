using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols
{
    class NullSymbol : Symbol
    {
        public NullSymbol(SyntaxToken xoToken) : base(xoToken)
        {
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}
