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
                        WITH _X AS
                        (
                            SELECT * FROM APMART_FPVIEWS..FPC_SERVICE LIMIT 100
                        )
                        SELECT  
                        *
                        FROM
                            APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            _X
                            ON _X.SVC_IDNTY = svc.SVC_IDNTY   
                ");


        static void Main(string[] args)
        {
            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);



            Console.WriteLine();
        }
    }
}
