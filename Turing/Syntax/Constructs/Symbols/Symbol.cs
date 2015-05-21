﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols
{
    class Symbol : SyntaxNode
    {
        public String Alias { get; set; }

        public Symbol(String xsRawText) : base (SyntaxKind.IdentifierToken, xsRawText)
        {
        }
    }
}
