using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;
using Turing.Syntax.Collections;

namespace Turing.Parser
{
    public class ParsingContext
    {
        public SyntaxNode CurrentNode;
        public SyntaxNode NewNode;
        public SyntaxTokenList List;

        public ParsingContext(SyntaxNode xoCurrentNode, SyntaxTokenList xoList)
        {
            CurrentNode = xoCurrentNode;
            List = xoList;
        }
    }
}
