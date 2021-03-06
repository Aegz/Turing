﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Syntax;
using Turing.Parser;
using Turing.Lexer;

namespace NetezzaParseTests
{
    [TestClass]
    public class ParenthesisTests
    {
        [TestMethod]
        public void Parse1ColumnParenthesised()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        (*)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(oColList.Count, 1);

            foreach (SyntaxNode oChild in oColList)
            {
                Assert.AreEqual(oChild.ExpectedType, SyntaxKind.OpenParenthesisToken);
                SyntaxNode oColInner = oChild[0];
                Assert.IsTrue(
                    oColInner.ExpectedType == SyntaxKind.StarToken || 
                    oColInner.ExpectedType == SyntaxKind.IdentifierTableSymbol);
            }
        }

        [TestMethod]
        public void ParseMultipleColumnParenthesised()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        (col1), (col2), (col3)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(3, oColList.Count);

            foreach (SyntaxNode oChild in oColList)
            {
                Assert.AreEqual(oChild.ExpectedType, SyntaxKind.OpenParenthesisToken);
                SyntaxNode oColInner = oChild[0];
                Assert.IsTrue(
                    oColInner.ExpectedType == SyntaxKind.StarToken ||
                    oColInner.ExpectedType == SyntaxKind.IdentifierColumnSymbol ||
                    oColInner.ExpectedType == SyntaxKind.IdentifierTableSymbol);
            }
        }

        [TestMethod]
        public void ParseParenthesisedExpression()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */
                        SELECT
                            col1, col2
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc
                        WHERE (svc.MKT_PROD_CD = 'MOB PT' AND svc.SVC_STAT_CD<> 'C')    
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            SyntaxNode oParenthesis = oWhere[0];
            Assert.AreEqual(oParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oParenthesis[0];
            Assert.AreEqual(oAND.ExpectedType, SyntaxKind.AndKeyword);

        }

        [TestMethod]
        public void ParseParenthesisedExpressionAND()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */
                        SELECT
                            col1, col2
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc
                        WHERE (svc.MKT_PROD_CD = 'MOB PT' AND svc.SVC_STAT_CD<> 'C') AND (svc.SVC_IDNTY <> '0415783039')   
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);


            // Test that there is a select keyword in that subquery
            SyntaxNode oFirstAND = oWhere[0];
            Assert.AreEqual(oFirstAND.ExpectedType, SyntaxKind.AndKeyword);

            SyntaxNode oParenthesis = oFirstAND[0];
            Assert.AreEqual(oParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);

            // Test that there is a select keyword in that subquery
            SyntaxNode oSecondParenthesis = oFirstAND[1];
            Assert.AreEqual(oSecondParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);
        }

        [TestMethod]
        public void ParseCascadedParenthesis()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */
                        SELECT
                            col1, col2
                        FROM
                            APMART_FP.ADMIN.FPC_SERVICE svc
                        WHERE ((svc.MKT_PROD_CD = 'MOB PT') AND svc.SVC_STAT_CD<> 'C') AND (svc.SVC_IDNTY <> '0415783039')   
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oFirstAND = oWhere[0];
            Assert.AreEqual(oFirstAND.ExpectedType, SyntaxKind.AndKeyword);

            SyntaxNode oParenthesis = oFirstAND[0];
            Assert.AreEqual(oParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);

            // Inner AND
            SyntaxNode oInnerAND = oParenthesis[0];
            Assert.AreEqual(oInnerAND.ExpectedType, SyntaxKind.AndKeyword);

            // LEFT SIDE of INNER AND should be parenthesis
            SyntaxNode oInnerParenthesis = oInnerAND[0];
            Assert.AreEqual(oParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);

            // Test that there is a select keyword in that subquery
            SyntaxNode oSecondParenthesis = oFirstAND[1];
            Assert.AreEqual(oSecondParenthesis.ExpectedType, SyntaxKind.OpenParenthesisToken);
        }

    }
}
