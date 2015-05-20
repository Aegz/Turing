using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs.Keywords;

namespace Turing.Parser
{
    class SyntaxNodeFactory
    {
        public delegate SyntaxNode GeneratorFunction(SyntaxTokenList xoGivenList);

        public static SyntaxNode test(SyntaxKind xeKind, SyntaxTokenList xoList)
        {
            // 1. Construct a Node based off the kind
            SelectSyntaxNode oSelect = new SelectSyntaxNode();

            // 2. Depth first traversal based on the node itself
            oSelect.TryConsumeList(xoList);

            // 3. Return this object after it consumed all the tokens it could
            return oSelect;
        }
    }
}
