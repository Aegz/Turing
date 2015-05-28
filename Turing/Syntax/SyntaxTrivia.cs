using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;

namespace Turing.Syntax
{
    public class SyntaxTrivia 
    {
        public String RawSQLText { get; set; }
        public SyntaxKind ExpectedType { get; set; }

        public List<StatusItem> Comments;

        public SyntaxTrivia(SyntaxKind xeType, String xsRawText) 
        {
            ExpectedType = xeType;
            RawSQLText = xsRawText;
            Comments = new List<StatusItem>();
        }

    }
}
