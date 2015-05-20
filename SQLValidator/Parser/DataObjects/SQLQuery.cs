using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.DataObjects
{
    public class SQLQuery 
    {
        protected List<SQLStatement> aoStatements;

        public List<SQLStatement> Statements
        {
            get
            {
                if (aoStatements == null)
                {
                    aoStatements = new List<SQLStatement>();
                }
                return aoStatements;
            }
            set { aoStatements = value; }
        }

        public SQLQuery()
        {
            // Always start with one
            Statements.Add(new SQLStatement());
        }

        public void AddStatement(SQLStatement xoChild)
        {
            //
            Statements.Add(xoChild);
        }

        public override String ToString()
        {
            // Recursive call on children to get their string values
            return (aoStatements != null ? String.Join("", Statements.Select((oVar) => oVar.ToString())) : "");
        }

    }
}