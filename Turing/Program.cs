using System;
using System.Collections.Generic;
using Turing.Lexer;
using Turing.Syntax;

namespace Turing
{
    class Program
    {
        static SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                    SELECT * FROM APAPP_CRMMP..MA504740005_COMM9729001
                    WHERE VARDATA3 = CASE WHEN VARDATA3 IS NOT NULL THEN 'MOBILE PHONE' ELSE '' END
                ");

        static void Main(string[] args)
        {
            // Initialises the Parser
            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect      = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            SyntaxNode oFrom        = oTemp.FindFirst(SyntaxKind.FromKeyword);
            SyntaxNode oInnerJoin   = oTemp.FindFirst(SyntaxKind.InnerJoinKeyword);
            SyntaxNode oWhere       = oTemp.FindFirst(SyntaxKind.WhereKeyword);

            // Temp to see some text out
            String sTemp = oTemp.ToString();
            Console.WriteLine();
        }
    }
}
