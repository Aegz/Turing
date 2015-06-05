using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaParseTests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void ParseMultipleFunctionInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                        svc_mkt_prod as mktprod, COALESCE(svc_idnty, 1, 10)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc        
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(2, oColumnList.Children.Count); // Should be 2

            SyntaxNode oColIdn = oColumnList.Children[0];
            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oColIdn.ExpectedType);

            SyntaxNode oFunction = oColumnList.Children[1];
            Assert.AreEqual(SyntaxKind.CoalesceKeyword, oFunction.ExpectedType);
            Assert.AreEqual(1, oFunction.Children.Count);
            Assert.AreEqual(3, oFunction.Children[0].Children.Count);
        }

        [TestMethod]
        public void ParseSingleFunctionInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                        COALESCE(svc_idnty, 1, 10)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc        
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oColumnList = oSelect.Children[0];
            Assert.AreEqual(1, oColumnList.Children.Count); 

            SyntaxNode oFunction = oColumnList.Children[0];
            Assert.AreEqual(SyntaxKind.CoalesceKeyword, oFunction.ExpectedType);
            Assert.AreEqual(1, oFunction.Children.Count);
            Assert.AreEqual(3, oFunction.Children[0].Children.Count);
        }



        [TestMethod]
        public void ParseSingleFunctionInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc     
                        WHERE MAX(svc.somecode) > 100   
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreEqual(1, oWhere.Children.Count);

            SyntaxNode oGreaterThan = oWhere.Children[0];
            Assert.AreEqual(SyntaxKind.GreaterThanToken, oGreaterThan.ExpectedType);

            SyntaxNode oFunc = oGreaterThan.Children[0];
            Assert.AreEqual(SyntaxKind.MaxKeyword, oFunc.ExpectedType);
        }

        [TestMethod]
        public void ParseMultipleFunctionInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc     
                        WHERE MAX(svc.somecode) > COUNT(svc_idnty)  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreEqual(1, oWhere.Children.Count);

            SyntaxNode oGreaterThan = oWhere.Children[0];
            Assert.AreEqual(SyntaxKind.GreaterThanToken, oGreaterThan.ExpectedType);

            SyntaxNode oFunc = oGreaterThan.Children[0];
            Assert.AreEqual(SyntaxKind.MaxKeyword, oFunc.ExpectedType);

            SyntaxNode oFunc2 = oGreaterThan.Children[1];
            Assert.AreEqual(SyntaxKind.CountKeyword, oFunc2.ExpectedType);
        }



        [TestMethod]
        public void ParseSingleFunctionInOn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            APMART_FPVIEWS..FPC_ACCOUNT acct
                            ON svc.acct-ID = acct.acct_id   
                            AND MAX(svc.somecode) > 100   
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreEqual(2, oWhere.Children.Count);

            SyntaxNode oGreaterThan = oWhere.Children[1];
            Assert.AreEqual(SyntaxKind.GreaterThanToken, oGreaterThan.ExpectedType);

            SyntaxNode oFunc = oGreaterThan.Children[0];
            Assert.AreEqual(SyntaxKind.MaxKeyword, oFunc.ExpectedType);
        }

        [TestMethod]
        public void ParseMultipleFunctionInOn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT  
                            svc_idnty
                        FROM
                            APSHARE_FPVIEWS..FPC_SERVICE svc     
                        INNER JOIN
                            APMART_FPVIEWS..FPC_ACCOUNT acct
                            ON svc.acct-ID = acct.acct_id
                            AND MAX(svc.somecode) > COUNT(svc_idnty)  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreEqual(2, oWhere.Children.Count);

            SyntaxNode oGreaterThan = oWhere.Children[1];
            Assert.AreEqual(SyntaxKind.GreaterThanToken, oGreaterThan.ExpectedType);

            SyntaxNode oFunc = oGreaterThan.Children[0];
            Assert.AreEqual(SyntaxKind.MaxKeyword, oFunc.ExpectedType);

            SyntaxNode oFunc2 = oGreaterThan.Children[1];
            Assert.AreEqual(SyntaxKind.CountKeyword, oFunc2.ExpectedType);
        }
    }
}
