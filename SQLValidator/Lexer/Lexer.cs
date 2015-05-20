using SQLValidator.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLValidator.Lexer
{
    class Lexer
    {
        // A window designed to mimic a Queue but has the flexibility
        // to allow peeking ahead
        private SlidingTextWindow oWindow;

        private LexerContext Context { get; set; }

        public Lexer(String xsGivenText)
        {
            oWindow = new SlidingTextWindow(xsGivenText);
        }

        
    }
}
