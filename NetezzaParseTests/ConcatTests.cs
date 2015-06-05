using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaParseTests
{
    [TestClass]
    public class ConcatTests
    {
        [TestMethod]
        public void ParseSimpleWhereConcat()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty = '0415783039' || 'PEW' || 'test'  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(2, oWhere[0].Count);
            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.BarBarToken, oWhere[0][1].ExpectedType);
            Assert.AreEqual(3, oWhere[0][1].Count);
        }

        [TestMethod]
        public void ParseSimpleColumnConcat()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT '+61' || svc_idnty  FROM FPC_SERVICE
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList[0];
            Assert.AreEqual(2, oBarBar.Count);
            Assert.AreEqual(SyntaxKind.BarBarToken, oBarBar.ExpectedType);
        }

        [TestMethod]
        public void ParseInvalidColumnConcat()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT '+61' ||  FROM FPC_SERVICE
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList[0];
            Assert.AreEqual(2, oBarBar.Count);
            Assert.AreEqual(SyntaxKind.BarBarToken, oBarBar.ExpectedType);
        }

        [TestMethod]
        public void ParseInvalidColumnPreConcat()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT || svc.svc_idnty FROM FPC_SERVICE
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList[0];
            Assert.AreEqual(2, oBarBar.Count);
            Assert.AreEqual(SyntaxKind.BarBarToken, oBarBar.ExpectedType);
        }

        [TestMethod]
        public void ParseComplexColumnConcat()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT '+61' || SUBSTR(svc_idnty, 1, 8) || '0'  FROM FPC_SERVICE  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList[0];
            Assert.AreEqual(3, oBarBar.Count);
            Assert.AreEqual(SyntaxKind.BarBarToken, oBarBar.ExpectedType);
            Assert.AreEqual(SyntaxKind.LiteralToken, oBarBar[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.SubStringKeyword, oBarBar[1].ExpectedType);
            Assert.AreEqual(SyntaxKind.LiteralToken, oBarBar[2].ExpectedType);
        }
    }
}
