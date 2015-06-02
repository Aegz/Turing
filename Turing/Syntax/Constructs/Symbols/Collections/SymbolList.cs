﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Symbols.Collections
{
    public class SymbolList : SyntaxNode
    {
        public SymbolList() : this (new SyntaxToken(SyntaxKind.ColumnListNode, ""))
        {
        }

        public SymbolList(SyntaxToken xoToken) : base (xoToken, NodeStrategyFactory.SYMBOL_LIST_STRATEGY)
        {

        }

        public override string GetChildString()
        {
            return String.Join(", ", Children);
        }
    }
}
