using SQLValidator.Lexer.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLValidator.Lexer
{
    class ParserContext
    {
        public SQLQuery Query;

        public ParserContext()
        {
            Query = new SQLQuery();
        }

        public SQLStatement CurrentStatement
        {
            get
            {
                // Initialise if necessary
                if (Query.Statements.Count == 0)
                {
                    Query.Statements.Add(new SQLStatement());
                }

                // Return the last statement
                return Query.Statements[Query.Statements.Count - 1];
            }
            set
            {
                // Append new statments when assigned to
                Query.AddStatement(value);
            }
        }

    }
}
