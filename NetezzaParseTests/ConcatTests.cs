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
            Assert.AreEqual(1, oWhere.Children.Count);
            Assert.AreEqual(2, oWhere.Children[0].Children.Count);
            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere.Children[0].Children[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.BarBarToken, oWhere.Children[0].Children[1].ExpectedType);
            Assert.AreEqual(3, oWhere.Children[0].Children[1].Children.Count);
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
            Assert.AreEqual(2, oSelect.Children.Count);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(1, oColumnList.Children.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList.Children[0];
            Assert.AreEqual(2, oBarBar.Children.Count);
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
            Assert.AreEqual(2, oSelect.Children.Count);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(1, oColumnList.Children.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList.Children[0];
            Assert.AreEqual(2, oBarBar.Children.Count);
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
            Assert.AreEqual(2, oSelect.Children.Count);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(1, oColumnList.Children.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList.Children[0];
            Assert.AreEqual(2, oBarBar.Children.Count);
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
            Assert.AreEqual(2, oSelect.Children.Count);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(1, oColumnList.Children.Count);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oBarBar = oColumnList.Children[0];
            Assert.AreEqual(3, oBarBar.Children.Count);
            Assert.AreEqual(SyntaxKind.BarBarToken, oBarBar.ExpectedType);
            Assert.AreEqual(SyntaxKind.LiteralToken, oBarBar.Children[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.SubStringKeyword, oBarBar.Children[1].ExpectedType);
            Assert.AreEqual(SyntaxKind.LiteralToken, oBarBar.Children[2].ExpectedType);
        }
    }
}
