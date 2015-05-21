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
                            APSHARE_FPVIEWS..FPC_SERVICE svc   

                ");


        static void Main(string[] args)
        {
            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            SyntaxNode oTemp = oParser.ParseTree();

            Console.WriteLine();
        }
    }
}
