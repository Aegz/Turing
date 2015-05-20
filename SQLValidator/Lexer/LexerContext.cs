using SQLValidator.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLValidator.Lexer
{
    class LexerContext 
    {
        public List<SQLToken> Items;

        public LexerContext()
        {
            Items = new List<SQLToken>();
        }

    

    }
}
