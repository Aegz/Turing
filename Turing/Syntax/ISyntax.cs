using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax
{
    public interface ISyntax
    {
        // Core properties that need to exist
        String RawSQLText { get; set; }
        SyntaxKind ExpectedType { get; set; }


        List<SyntaxTrivia> LeadingTrivia { get; set; }
        List<SyntaxTrivia> TrailingTrivia { get; set; }

        // Something to figure out what type it actually is
        Boolean IsNode();
    }
}
