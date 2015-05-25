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
                        (
                            SELECT * FROM FPC_SERVICE
                        )
                ");


        static void Main(string[] args)
        {
            // Initialises the Parser
            Parser.SyntaxParser oParser = new Parser.SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Temp to see some text out
            String sTemp = oTemp.ToString();
            Console.WriteLine();

        }
    }
}
