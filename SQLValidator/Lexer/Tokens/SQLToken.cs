using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.Tokens
{
    public class SQLToken
    {
        public SyntaxKind ExpectedType { get; set; }

        public String RawSQLText { get; set; }


        public SQLToken(SyntaxKind xeType, String xsRawText)
        {
            RawSQLText = xsRawText;
            ExpectedType = xeType;
        }


    }
}