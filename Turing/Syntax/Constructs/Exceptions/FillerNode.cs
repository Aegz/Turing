using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Exceptions
{
    class FillerNode : SyntaxNode
    {
        //SyntaxKind eExpectedKind;

        public FillerNode(SyntaxKind xeKind, String xsRawText) : base(new SyntaxToken(xeKind, xsRawText))
        {
            //eExpectedKind = xeKind;
            //xeKind = SyntaxKind.MissingNode;
        }


    }
}
