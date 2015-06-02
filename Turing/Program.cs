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
                            APMART_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            (
                                SELECT * FROM APSHARE_FP..WR02052_OMR_BASE 
                            ) omrbase
                        ON svc.SVC_IDNTY = omrbase.SVC_IDNTY 
                        LEFT JOIN
                            APSHARE_FP..WR02052_OM_REWARDS omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY AND
                           svc.MKT_PROD_CD = 'MOB PT'   
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
