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
    public class Symbol : SyntaxNodeWithOneChild
    {
        private String sAlias;
        // The Alias can be buried so we need to go and get one
        public String Alias
        {
            get
            {
                // If this alias is blank
                if (String.IsNullOrWhiteSpace(sAlias))
                {
                    // Check the children for any alias
                    String sReturn = String.Empty;
                    foreach (SyntaxNode oChild in Children)
                    {
                        if (typeof(Symbol).IsAssignableFrom(oChild.GetType()))
                        {
                            sReturn += ((Symbol)oChild).Alias;
                        }
                    }
                    return sReturn;
                }

                return sAlias;
            }
            set
            {
                sAlias = value;
            }
        }

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
