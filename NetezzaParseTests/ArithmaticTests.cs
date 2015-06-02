using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Syntax;
using Turing.Parser;

namespace NetezzaParseTests
{
    [TestClass]
    public class ArithmaticTests
    {
        [TestMethod]
        public void TestMultiplicationInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty * 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.FindFirst(SyntaxKind.ColumnListNode);
            Assert.AreNotEqual(oColumnList, null);
            SyntaxNode oStarExpression = oSelect.FindFirst(SyntaxKind.StarToken);
            Assert.AreNotEqual(oStarExpression, null);

            SyntaxNode oLeft = oStarExpression.Children[0];
            SyntaxNode oRight = oStarExpression.Children[1];

            Assert.AreEqual(oLeft.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            Assert.AreEqual(oLeft.Children[0].ExpectedType, SyntaxKind.IdentifierColumnSymbol);
            Assert.AreEqual(oRight.ExpectedType,SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void TestSubtractInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty - 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.FindFirst(SyntaxKind.ColumnListNode);
            Assert.AreNotEqual(oColumnList, null);
            SyntaxNode oStarExpression = oSelect.FindFirst(SyntaxKind.MinusToken);
            Assert.AreNotEqual(oStarExpression, null);

            SyntaxNode oLeft = oStarExpression.Children[0];
            SyntaxNode oRight = oStarExpression.Children[1];

            Assert.AreEqual(oLeft.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            Assert.AreEqual(oLeft.Children[0].ExpectedType, SyntaxKind.IdentifierColumnSymbol);
            Assert.AreEqual(oRight.ExpectedType, SyntaxKind.NumericToken);
        }
   
        [TestMethod]
        public void TestAdditionInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty + 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.FindFirst(SyntaxKind.ColumnListNode);
            Assert.AreNotEqual(oColumnList, null);
            SyntaxNode oStarExpression = oSelect.FindFirst(SyntaxKind.PlusToken);
            Assert.AreNotEqual(oStarExpression, null);

            SyntaxNode oLeft = oStarExpression.Children[0];
            SyntaxNode oRight = oStarExpression.Children[1];

            Assert.AreEqual(oLeft.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            Assert.AreEqual(oRight.ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void TestDivisionInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty / 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.FindFirst(SyntaxKind.ColumnListNode);
            Assert.AreNotEqual(oColumnList, null);
            SyntaxNode oStarExpression = oSelect.FindFirst(SyntaxKind.SlashToken);
            Assert.AreNotEqual(oStarExpression, null);

            SyntaxNode oLeft = oStarExpression.Children[0];
            SyntaxNode oRight = oStarExpression.Children[1];

            Assert.AreEqual(oLeft.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            Assert.AreEqual(oRight.ExpectedType, SyntaxKind.NumericToken);
        }



    }
}
