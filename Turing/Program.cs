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
                            (col1), (col2)
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc 
                        INNER JOIN
                            APSHARE_FP..WR02052_OMR_BASE omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY   
                        WHERE (svc.MKT_PROD_CD = 'MOB PT' AND svc.SVC_STAT_CD<> 'C') AND (svc.SVC_IDNTY <> '0415783039') 
                ");

        static void Main(string[] args)
        {
            // Initialises the Parser
            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            SyntaxNode oFrom = oTemp.FindFirst(SyntaxKind.FromKeyword);
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);

            // Temp to see some text out
            String sTemp = oTemp.ToString();
            Console.WriteLine();
        }
    }
}
