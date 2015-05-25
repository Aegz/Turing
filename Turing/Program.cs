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
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc
                        WHERE (svc.MKT_PROD_CD = 'MOB PT' AND svc.SVC_STAT_CD <> 'C') AND (svc.SVC_IDNTY <> '0415783039')
                ");

        static void Main(string[] args)
        {
            // Initialises the Parser
            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oFound = oTemp.FindFirst(SyntaxKind.SelectKeyword);

            // Temp to see some text out
            String sTemp = oTemp.ToString();
            Console.WriteLine();
        }
    }
}
