using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax.Collections;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class SubquerySymbol : TableSymbol
    {
        public SubquerySymbol(SyntaxToken xoToken) : base (xoToken, NodeStrategyFactory.SUBQUERY_STRATEGY)
        {
        }

        public override string ToString()
        {
            String sReturnString = String.Empty;
            switch (Token.ExpectedType)
            {
                case SyntaxKind.OpenParenthesisToken:
                    sReturnString = "(" + GetChildString() + ")";
                    break;
                default:
                    sReturnString = GetChildString();
                    break;
            }

            return sReturnString + (String.IsNullOrEmpty(Alias) ? String.Empty : " " + Alias);
        }
    }
}
