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
                            col2
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc1
                        INNER JOIN 
                            APMART_FPVIEWS..FPC_PEW svc2
                        ON 
                            svc1.TEMP = svc2.TEMP
                        WHERE (svc1.svc_idnty = '0415783039' AND svc1.svc_idnty1 = '0415783039')
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
