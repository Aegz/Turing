using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;
using Turing.Syntax.Constructs.Keywords;

namespace Turing.Parser
{
    class SyntaxTokenToNodeConverter
    {
        public static SyntaxNode ConvertTokenToNode(SyntaxToken xoToken)
        {
            switch(xoToken.ExpectedType)
            {
                case SyntaxKind.SelectKeyword:
                    return new SelectSyntaxNode();
                case SyntaxKind.FromKeyword:
                    return new FromSyntaxNode();

                default:
                    return new SyntaxNode(xoToken);
            }
        } 
    }
}
