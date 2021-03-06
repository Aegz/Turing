﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Parser;
using Turing.Syntax;
using Turing.Lexer;

namespace NetezzaParseTests
{
    [TestClass]
    public class WhereOnTests
    {
        [TestMethod]
        public void ParseBasicWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE svc.MKT_PROD_CD = 'MOB PT'
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oEqual = oWhere.FindFirst(SyntaxKind.EqualsToken);
            Assert.AreNotEqual(oEqual, null);

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oEqual.Count);
            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oEqual[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.LiteralToken, oEqual[1].ExpectedType);
        }

        [TestMethod]
        public void ParseBadWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE     
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
    
            Assert.AreEqual(SyntaxKind.BooleanToken, oWhere[0].ExpectedType); // Check for the Filler node
        }

        [TestMethod]
        public void ParseWhereOr()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE svc.MKT_PROD_CD = 'MOB PT' OR svc.SVC_STAT_CD <> 'C'      
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oOR = oWhere.FindFirst(SyntaxKind.OrKeyword);
            Assert.IsTrue(oOR != null);

            // Test that the OR was generated properly (exactly 2 children)
            Assert.AreEqual(2, oOR.Count);
        }

        [TestMethod]
        public void ParseWhereOrTree()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE 
                            svc.MKT_PROD_CD = 'MOB PT' OR 
                            svc.SVC_STAT_CD <> 'C' or
                            svc.SVC_IDNTY = '0423234042'
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(oWhere, null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oOR = oWhere.FindFirst(SyntaxKind.OrKeyword);
            Assert.AreNotEqual(null, oOR);

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oOR.Count);
            Assert.AreEqual(2, oOR[0].Count); // Inner AND has 2 children
        }


        [TestMethod]
        public void ParseWhereAnd()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE svc.MKT_PROD_CD = 'MOB PT' AND svc.SVC_STAT_CD <> 'C'      
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oWhere.FindFirst(SyntaxKind.AndKeyword);
            Assert.IsTrue(oAND != null);

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oAND.Count);
        }

        [TestMethod]
        public void ParseWhereAndTree()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE 
                            svc.MKT_PROD_CD = 'MOB PT' AND 
                            svc.SVC_STAT_CD <> 'C' AND
                            svc.SVC_IDNTY = '0423234042'
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(oWhere, null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oWhere.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreNotEqual(null, oAND);

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oAND.Count);
            Assert.AreEqual(2, oAND[0].Count); // Inner AND has 2 children
        }


        [TestMethod]
        public void ParseBadAnd()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE AND svc.SVC_STAT_CD <> 'C'      
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oWhere.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreNotEqual(null, oAND);
            Assert.AreEqual(SyntaxKind.BooleanToken, oAND[0].ExpectedType); // Check for the Filler node

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oAND.Count);
        }

        [TestMethod]
        public void ParseEmptyAnd()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        WHERE AND    
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.IsTrue(oWhere != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oWhere.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreNotEqual(null, oAND);
            Assert.AreEqual(SyntaxKind.BooleanToken, oAND[0].ExpectedType); // Check for the Filler node

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oAND.Count);
        }



        [TestMethod]
        public void ParseOnAnd()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE
                        ) svc
                        INNER JOIN 
                            APSHARE_FP..FWR02052_OMR_BASE omr
                            ON 
                                svc.MKT_PROD_CD = 'MOB PT' AND 
                                svc.SVC_IDNTY = omr.SERVICE_NO  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oON = oTemp.FindFirst(SyntaxKind.OnKeyword);
            Assert.AreNotEqual(oON, null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oAND = oON.FindFirst(SyntaxKind.AndKeyword);
            Assert.AreNotEqual(oAND, null);
            Assert.AreEqual(oAND.RawSQLText, "AND");

            // Test that the AND was generated properly (exactly 2 children)
            Assert.AreEqual(2, oAND.Count);

            SyntaxNode oLeftEquals = oAND[0];
            Assert.AreEqual(oLeftEquals.ExpectedType, SyntaxKind.EqualsToken);

            SyntaxNode oLeftEqualsL = oLeftEquals[0];
            Assert.AreEqual(oLeftEqualsL.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            SyntaxNode oLeftEqualsR = oLeftEquals[1];
            Assert.AreEqual(oLeftEqualsR.ExpectedType, SyntaxKind.LiteralToken);


            SyntaxNode oRightEquals = oAND[1];
            Assert.AreEqual(oLeftEquals.ExpectedType, SyntaxKind.EqualsToken);
            SyntaxNode oRightEqualsL = oRightEquals[0];
            Assert.AreEqual(oRightEqualsL.ExpectedType, SyntaxKind.IdentifierTableSymbol);
            SyntaxNode oRightEqualsR = oRightEquals[1];
            Assert.AreEqual(oRightEqualsR.ExpectedType, SyntaxKind.IdentifierTableSymbol);
        }


    }
}
