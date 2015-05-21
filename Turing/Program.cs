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
                        *
                        FROM
                            APMART_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            APSHARE_FP..WR02052_OMR_BASE omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY 

                ");


        static void Main(string[] args)
        {
            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            SyntaxNode oTemp = oParser.ParseTree();

            String sTemp = oTemp.ToString();
            Console.WriteLine();

        }
    }
}
