using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax.Collections;

namespace Turing.Syntax.Constructs.Symbols.SingleChild
{
    class SubquerySymbol : TableSymbol
    {
        public SubquerySymbol(SyntaxToken xoToken) : base (xoToken)
        {
            ConsumableTypes.AddRange(new List<SyntaxKind>
            {
                { SyntaxKind.SelectKeyword }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xoList"></param>
        /// <returns></returns>
        public override bool CanProcessNextNode(SyntaxTokenList xoList)
        {
            // If we have a Open parenthesis starting node
            // And we just found a closing Token
            if (this.ExpectedType == SyntaxKind.OpenParenthesisToken &&
                xoList.PeekToken().ExpectedType == SyntaxKind.CloseParenthesisToken)
            {
                // Drop the close parenthesis
                xoList.PopToken();

                // Find an alias
                this.Alias = SyntaxNodeFactory.ScanAheadForAlias(xoList);

                // If the alias is blank
                if (String.IsNullOrEmpty(Alias))
                {
                    Alias = "<GEN/>";
                    InsertStatusMessage("No Alias assigned. One has been generated");
                }

                return true;
            }

            return base.CanProcessNextNode(xoList);
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
