﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class TableSymbol : Symbol
    {
        public TableSymbol(String xsRawText) : base (xsRawText)
        {
        }

        public override string ToString()
        {
            return RawSQLText + (Children.Count > 0 ? "." + base.GetChildString() : " " + Alias);
        }
    }
}