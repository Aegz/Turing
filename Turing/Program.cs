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
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty NOT IN ('0415783039')   
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
