using SQLValidator.Lexer.DataObjects.Attribute;
using SQLValidator.Lexer.DataObjects.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.DataObjects.Keyword
{
    public class SQLKeyword : KeywordOrExpression
    {
        protected List<SQLAttribute> aoAttributes;

        public List<SQLAttribute> Attributes
        {
            get
            {
                if (aoAttributes == null)
                {
                    aoAttributes = new List<SQLAttribute>();
                }
                return aoAttributes;
            }
            set { aoAttributes = value; }
        }

        public SQLKeyword(String xsRawSQL) : base(xsRawSQL)
        {

        }
    }
}