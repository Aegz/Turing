using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.DataObjects.Node
{
    public class KeywordOrExpression : IEnumerable<KeywordOrExpression>
    {
        public String RawSQLText { get; set; }

        protected String PrettyText { get; set; }

        private List<KeywordOrExpression> aoChildren;

        public List<KeywordOrExpression> Children
        {
            get
            {
                if (aoChildren == null)
                {
                    aoChildren = new List<KeywordOrExpression>();
                }
                return aoChildren;
            }
            set { aoChildren = value; }
        }


        
        public KeywordOrExpression()
        {

        }

        public KeywordOrExpression(String xsRawText)
        {
            RawSQLText = xsRawText;
        }


        public virtual void AddChild(KeywordOrExpression xoChild)
        {
            aoChildren.Add(xoChild);
        }

        public String ToString(String xsGivenFormat = "")
        {
            // Core (Generic)
            return PrettyText + String.Join("", Children.Select((oNode) => oNode.ToString(xsGivenFormat)));
        }

        IEnumerator<KeywordOrExpression> IEnumerable<KeywordOrExpression>.GetEnumerator()
        {
            return aoChildren.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return aoChildren.GetEnumerator();
        }
    }
}