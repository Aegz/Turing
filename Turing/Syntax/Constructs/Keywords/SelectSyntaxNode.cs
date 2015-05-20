using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Constructs.Symbols;

namespace Turing.Syntax.Constructs.Keywords
{
    class SelectSyntaxNode : SyntaxNode
    {
        protected List<Symbol> aoColumns;
        public List<Symbol> Columns
        {
            get
            {
                if (aoColumns == null)
                {
                    aoColumns = new List<Symbol>();
                }

                return aoColumns;
            }
        }

    }
}
