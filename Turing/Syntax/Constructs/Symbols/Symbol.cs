using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Strategies;

namespace Turing.Syntax.Constructs.Symbols
{
    /// <summary>
    /// Symbol is a node that can only have 1 child. Will often represent
    /// a leaf
    /// </summary>
    class Symbol : SyntaxNodeWithOneChild
    {
        public String Alias { get; set; }

        public Symbol(SyntaxToken xoToken, NodeStrategy xoStrategy) : base(xoToken, xoStrategy)
        {
        }

        public Symbol(SyntaxToken xoToken) : this(xoToken, NodeStrategyFactory.FactoryCreateStrategy(xoToken.ExpectedType))
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
