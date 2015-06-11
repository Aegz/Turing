using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;

namespace Turing.Syntax
{
    public class SyntaxTrivia : ISyntax
    {
        public String RawSQLText { get; set; }
        public SyntaxKind ExpectedType { get; set; }

        public List<StatusItem> Comments;

        // Leading/Trailing Trivia on this returns a list with this object
        public List<SyntaxTrivia> LeadingTrivia
        {
            get
            {
                return new List<SyntaxTrivia>() { this };
            }
            set
            {

            }
        }
        public List<SyntaxTrivia> TrailingTrivia
        {
            get
            {
                return new List<SyntaxTrivia>() { this };
            }
            set
            {

            }
        }

        public SyntaxTrivia(SyntaxKind xeType, String xsRawText) 
        {
            ExpectedType = xeType;
            RawSQLText = xsRawText;
            Comments = new List<StatusItem>();
        }

        // This is not a node
        public Boolean IsNode()
        {
            return false;
        }
    }
}
