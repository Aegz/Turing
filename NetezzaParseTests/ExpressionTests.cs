using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaParseTests
{
    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void ParseMultiplication()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty = 2 * 4     
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
            Assert.AreEqual(SyntaxKind.StarToken, oWhere[0][1].ExpectedType);
        }

        [TestMethod]
        public void ParseIsNull()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty IS NULL  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(SyntaxKind.IsKeyword, oWhere[0].ExpectedType);
            Assert.AreEqual(2, oWhere[0].Count);

            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.NullKeyword,           oWhere[0][1].ExpectedType);
        }

        [TestMethod]
        public void ParseIsNotNull()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty IS NOT NULL  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(SyntaxKind.IsKeyword, oWhere[0].ExpectedType);
            Assert.AreEqual(2, oWhere[0].Count);

            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.NotKeyword, oWhere[0][1].ExpectedType);

            Assert.AreEqual(1, oWhere[0][1].Count);
            Assert.AreEqual(SyntaxKind.NullKeyword, oWhere[0][1][0].ExpectedType);
        }

        [TestMethod]
        public void ParseIN()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty IN ('0415783039')  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(SyntaxKind.InKeyword, oWhere[0].ExpectedType);
            Assert.AreEqual(2, oWhere[0].Count);

            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.OpenParenthesisToken, oWhere[0][1].ExpectedType);
            Assert.AreEqual(1, oWhere[0][1].Count);
        }


        [TestMethod]
        public void ParseNotIN()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty IN ('0415783039')  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(SyntaxKind.InKeyword, oWhere[0].ExpectedType);
            Assert.AreEqual(2, oWhere[0].Count);

            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.OpenParenthesisToken, oWhere[0][1].ExpectedType);
            Assert.AreEqual(1, oWhere[0][1].Count);
        }

        [TestMethod]
        public void ParseINMultipleItems()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT * FROM FPC_SERVICE
                        WHERE svc_idnty IN ('0415783039', '0422783234', '123121232')  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();
            
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);
            Assert.AreEqual(SyntaxKind.InKeyword, oWhere[0].ExpectedType);
            Assert.AreEqual(2, oWhere[0].Count);

            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oWhere[0][0].ExpectedType);
            Assert.AreEqual(SyntaxKind.ColumnListNode, oWhere[0][1].ExpectedType);
            Assert.AreEqual(3, oWhere[0][1].Count);
        }
    }
}
