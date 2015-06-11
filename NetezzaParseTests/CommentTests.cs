using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Syntax;
using Turing.Parser;
using Turing.Lexer;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Symbols;

namespace NetezzaParseTests
{
    [TestClass]
    public class CommentTests
    {

        [TestMethod]
        public void ParseBadColumnCommentFROM()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"
                        SELECT  
                        * --bad comment FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc      
            ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            SyntaxNode oFrom = oSelect.FindFirst(SyntaxKind.FromKeyword);
            Assert.AreNotEqual(oFrom, null);

            SyntaxNode oTableIdn = oFrom.FindFirst(SyntaxKind.IdentifierTableSymbol);
            Assert.AreNotEqual(oTableIdn, null);
        }

        [TestMethod]
        public void ParseBadColumnCommentFROMExtended()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"
                        SELECT * --bad comment FROM APSHARE_FPVIEWS..FPC_SERVICE svc      
            ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            SyntaxNode oFrom = oSelect.FindFirst(SyntaxKind.FromKeyword);
            Assert.AreNotEqual(oFrom, null);

            SyntaxNode oTableIdn = oFrom.FindFirst(SyntaxKind.IdentifierTableSymbol);
            Assert.AreNotEqual(oTableIdn, null);
        }


        [TestMethod]
        public void ParseBadColumnCommentWHERE()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"
                        SELECT 
                        * --bad comment 
                        FROM APSHARE_FPVIEWS..FPC_SERVICE svc    
                        -- WHERE x  
            ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            SyntaxNode oFrom = oSelect.FindFirst(SyntaxKind.FromKeyword);
            Assert.AreNotEqual(oFrom, null);

            SyntaxNode oWhere = oSelect.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreEqual(oWhere, null); // Should not be a where, They probably wanted this commented out
        }


    }
}
