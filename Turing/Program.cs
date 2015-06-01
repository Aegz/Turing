﻿using System;
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
                        svc_mkt_prod as mktprod, COALESCE(svc_idnty, 1, 10)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc           
                ");

        static void Main(string[] args)
        {
            SyntaxKind xeKind = SyntaxKindUtilities.GetKindFromString("");
            String xsKind = SyntaxKindUtilities.GetStringFromKind(SyntaxKind.BooleanToken);
            // Initialises the Parser
            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect  = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            SyntaxNode oFrom    = oTemp.FindFirst(SyntaxKind.FromKeyword);
            SyntaxNode oInnerJoin  = oTemp.FindFirst(SyntaxKind.InnerJoinKeyword);
            SyntaxNode oWhere   = oTemp.FindFirst(SyntaxKind.WhereKeyword);

            // Temp to see some text out
            String sTemp = oTemp.ToString();
            Console.WriteLine();
        }
    }
}
