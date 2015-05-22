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

        public Symbol(SyntaxToken xoToken) : base(xoToken)
        {
        }

        public Symbol() : this(SyntaxToken.NULL_TOKEN)
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
