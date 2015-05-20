using SQLValidator.Lexer.DataObjects;
using SQLValidator.Lexer.DataObjects.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLValidator.Lexer.DataObjects
{
    class ExpressionOrConst : KeywordOrExpression
    {
        public ExpressionOrConst Parent { get; set; }

        public String Value { get; set; }

        public ExpressionOrConst(String xsValue)
        {
            // Set the tag and default the value
            Value = xsValue;
        }

        public override String ToString()
        {
            // Recursive call on children to get their string values
            return Value;
        }
    }
}
