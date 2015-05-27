using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Syntax;
using Turing.Parser;
using System.Collections.Generic;
using Turing.Lexer;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Symbols.Collections;
using Turing.Syntax.Constructs.Symbols;

namespace NetezzaParseTests
{
    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void TestSelectFromDBTable()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oFrom = oSelect.FindFirst(SyntaxKind.FromKeyword);
            Assert.IsTrue(oFrom != null);

            SyntaxNode oTableIdn = oFrom.FindFirst(SyntaxKind.IdentifierToken);
            Assert.IsTrue(oTableIdn != null);
        }

        [TestMethod]
        public void TestSelectColumnAlias()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        col1 AS c2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oTableIdn = oSelect.Children[0];
            Assert.AreEqual(1, oTableIdn.Children.Count); // Should be 1 column
        }

        [TestMethod]
        public void TestSelectMultipleColumns()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        col1, col2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            SyntaxNode oTableIdn = oSelect.Children[0];
            Assert.AreEqual(2, oTableIdn.Children.Count); // Should be 2 columns
        }

        [TestMethod]
        public void TestSubqueryTree()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        (
                            SELECT * FROM FPC_SERVICE    
                        ) svc
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.AreEqual(oTemp.GetType(), typeof(QuerySyntaxNode));

            // Test that a subquery type node was built
            SyntaxNode oSubQuery = oTemp.FindFirst(SyntaxKind.OpenParenthesisToken);
            Assert.IsTrue(oSubQuery != null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oSelect = oSubQuery.FindFirst(SyntaxKind.SelectKeyword);
            Assert.IsTrue(oSelect != null);

            // Test that the identifier was generated properly
            SyntaxNode oTableIdn = oSelect.FindFirst(SyntaxKind.IdentifierToken);
            Assert.IsTrue(oTableIdn != null);


        }

        [TestMethod]
        public void TestAnd()
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
            Assert.AreEqual(oAND.Children.Count, 2);
        }

        [TestMethod]
        public void TestBasicJoinTree()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                            APMART_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            APSHARE_FP..WR02052_OMR_BASE omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY        
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Test that a subquery type node was built
            SyntaxNode oFrom = oTemp.FindFirst(SyntaxKind.FromKeyword);
            Assert.IsTrue(oFrom != null);

            SyntaxNode oInnerJoin = oFrom.Children[0];
            Assert.AreEqual(SyntaxKind.InnerJoinKeyword, oInnerJoin.ExpectedType);

            // test children are correct
            Assert.AreEqual(oInnerJoin.Children[0].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oInnerJoin.Children[1].ExpectedType, SyntaxKind.IdentifierToken);
        }

        [TestMethod]
        public void TestJoinWithSubQuery()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                            APMART_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            (
                                SELECT * FROM APSHARE_FP..WR02052_OMR_BASE 
                            ) omrbase
                        ON svc.SVC_IDNTY = omrbase.SVC_IDNTY 
                        LEFT JOIN
                            APSHARE_FP..WR02052_OM_REWARDS omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY AND
                           svc.MKT_PROD_CD = 'MOB PT'       
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.AreEqual(oTemp.GetType(), typeof(QuerySyntaxNode));


            // Test that a subquery type node was built
            SyntaxNode oFrom = oTemp.FindFirst(SyntaxKind.FromKeyword);
            Assert.IsTrue(oFrom != null);

            SyntaxNode oLeftJoin = oFrom.Children[0];
            Assert.AreEqual(oLeftJoin.ExpectedType, SyntaxKind.LeftJoinKeyword);
            Assert.AreEqual(oLeftJoin.Children[1].ExpectedType, SyntaxKind.IdentifierToken);

            SyntaxNode oInnerJoin = oLeftJoin.Children[0];
            Assert.AreEqual(oInnerJoin.ExpectedType, SyntaxKind.InnerJoinKeyword);
            

            // test children are correct
            Assert.AreEqual(oInnerJoin.Children[0].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oInnerJoin.Children[1].ExpectedType, SyntaxKind.OpenParenthesisToken);
        }

        [TestMethod]
        public void TestJoinWithTwoSubQueries()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                            col1, col2
                        FROM
                            APMART_FPVIEWS..FPC_SERVICE svc  
                        INNER JOIN
                            (
                                SELECT * FROM APSHARE_FP..WR02052_OMR_BASE 
                            ) omrbase
                        ON svc.SVC_IDNTY = omrbase.SVC_IDNTY 
                        LEFT JOIN
                            (
                                SELECT * FROM APSHARE_FP..WR02052_OMR 
                            ) omr
                        ON svc.SVC_IDNTY = omr.SVC_IDNTY AND
                           svc.MKT_PROD_CD = 'MOB PT'       
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.AreEqual(oTemp.GetType(), typeof(QuerySyntaxNode));

            // Test that a subquery type node was built
            SyntaxNode oFrom = oTemp.FindFirst(SyntaxKind.FromKeyword);
            Assert.IsTrue(oFrom != null);

            SyntaxNode oLeftJoin = oFrom.Children[0];
            Assert.AreEqual(SyntaxKind.LeftJoinKeyword, oLeftJoin.ExpectedType);
            Assert.AreEqual("omr", ((Symbol)oLeftJoin.Children[1]).Alias);

            SyntaxNode oInnerJoin = oLeftJoin.Children[0];
            Assert.AreEqual(SyntaxKind.InnerJoinKeyword, oInnerJoin.ExpectedType);
            
            // test children are correct
            Assert.AreEqual(SyntaxKind.IdentifierToken, oInnerJoin.Children[0].ExpectedType);
            Assert.AreEqual("svc", ((Symbol)oInnerJoin.Children[0]).Alias);

            Assert.AreEqual(SyntaxKind.OpenParenthesisToken, oInnerJoin.Children[1].ExpectedType);
            Assert.AreEqual("omrbase", ((Symbol)oInnerJoin.Children[1]).Alias);
        }

    }
}
