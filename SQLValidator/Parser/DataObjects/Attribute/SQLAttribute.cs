using SQLValidator.Lexer.DataObjects.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.DataObjects.Attribute
{
    /// <summary>
    /// Inherits from KeywordOrExpression since it needs to be traversed 
    /// in a lot of cases 
    /// eg. FROM
    /// </summary>
    public class SQLAttribute : KeywordOrExpression
    {

    }
}