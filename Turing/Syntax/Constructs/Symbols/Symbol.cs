using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax.Constructs.Symbols
{
    class Symbol : SyntaxNodeWithOneChild
    {
        public String Alias { get; set; }

        public Symbol(String xsRawText) : base (SyntaxKind.IdentifierToken, xsRawText)
        {
        }

        public Symbol() : this("")
        {

        }

        public String GetAlias()
        {
            return String.IsNullOrWhiteSpace(Alias) ? 
                String.Join("", Children.Select((oNode) => ((Symbol)oNode).GetAlias())) : 
                Alias;
        }
    }
}
