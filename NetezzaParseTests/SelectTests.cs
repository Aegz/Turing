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
    public class SelectTests
    {
        [TestMethod]
        public void ParseSelectFromDBTable()
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

            SyntaxNode oTableIdn = oFrom.FindFirst(SyntaxKind.IdentifierTableSymbol);
            Assert.IsTrue(oTableIdn != null);
        }

        [TestMethod]
        public void ParseBadColumnComment()
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
        public void ParseSelectColumnAlias()
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

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count); // Should be 1 column
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oLiteralColumn = oColumnList[0];
            Assert.AreEqual(SyntaxKind.IdentifierTableSymbol, oLiteralColumn.ExpectedType);
            Assert.AreEqual(typeof(Symbol), oLiteralColumn.GetType()); // Table is the top level then col
            Assert.AreEqual("c2", ((Symbol)oLiteralColumn).Alias);
        }

        [TestMethod]
        public void ParseStringLiteralColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        'TEST'
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count); // Should be 1 column
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType); 

            SyntaxNode oLiteralColumn = oColumnList[0];
            Assert.AreEqual(SyntaxKind.LiteralToken, oLiteralColumn.ExpectedType);
        }

        [TestMethod]
        public void ParseStringLiteralColumnWithAlias()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        'TEST' as var1
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);
            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);

            SyntaxNode oColumnList = oSelect[0];
            Assert.AreEqual(1, oColumnList.Count); // Should be 1 column
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColumnList.ExpectedType);

            SyntaxNode oLiteralColumn = oColumnList[0];
            Assert.AreEqual(typeof(Symbol), oLiteralColumn.GetType());
            Assert.AreEqual("var1", ((Symbol)oLiteralColumn).Alias);
        }

        [TestMethod]
        public void ParseSelectMultipleColumns()
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

            SyntaxNode oTableIdn = oSelect[0];
            Assert.AreEqual(2, oTableIdn.Count); // Should be 2 columns
        }


        [TestMethod]
        public void ParseSubqueryTree()
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
            SyntaxNode oSubQuery = oTemp.FindFirst(SyntaxKind.IdentifierSubQuerySymbol);
            Assert.AreNotEqual(oSubQuery, null);

            // Test that there is a select keyword in that subquery
            SyntaxNode oSelect = oSubQuery.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(oSelect, null);

            // Test that the identifier was generated properly
            SyntaxNode oTableIdn = oSelect.FindFirst(SyntaxKind.IdentifierTableSymbol);
            Assert.AreNotEqual(oTableIdn, null);


        }

        [TestMethod]
        public void ParseBasicJoinTree()
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

            SyntaxNode oInnerJoin = oFrom[0];
            Assert.AreEqual(3, oInnerJoin.Count);
            Assert.AreEqual(SyntaxKind.InnerJoinKeyword, oInnerJoin.ExpectedType);

            // test children are correct
            Assert.AreEqual(oInnerJoin[0].ExpectedType, SyntaxKind.IdentifierDatabaseSymbol);
            Assert.AreEqual(oInnerJoin[1].ExpectedType, SyntaxKind.IdentifierDatabaseSymbol);
        }

        [TestMethod]
        public void ParseJoinWithSubQuery()
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

            SyntaxNode oLeftJoin = oFrom[0];
            Assert.AreEqual(SyntaxKind.LeftJoinKeyword, oLeftJoin.ExpectedType);
            Assert.AreEqual(SyntaxKind.IdentifierDatabaseSymbol, oLeftJoin[1].ExpectedType);

            SyntaxNode oInnerJoin = oLeftJoin[0];
            Assert.AreEqual(oInnerJoin.ExpectedType, SyntaxKind.InnerJoinKeyword);
            

            // test children are correct
            Assert.AreEqual(oInnerJoin[0].ExpectedType, SyntaxKind.IdentifierDatabaseSymbol);
            Assert.AreEqual(oInnerJoin[1].ExpectedType, SyntaxKind.IdentifierSubQuerySymbol);
        }

        [TestMethod]
        public void ParseJoinWithTwoSubQueries()
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

            SyntaxNode oLeftJoin = oFrom[0];
            Assert.AreEqual(SyntaxKind.LeftJoinKeyword, oLeftJoin.ExpectedType);
            Assert.AreEqual(3, oLeftJoin.Count);
            Assert.AreEqual("omr", ((Symbol)oLeftJoin[1]).Alias);

            SyntaxNode oInnerJoin = oLeftJoin[0];
            Assert.AreEqual(3, oInnerJoin.Count);
            Assert.AreEqual(SyntaxKind.InnerJoinKeyword, oInnerJoin.ExpectedType);
            
            // test children are correct
            Assert.AreEqual(SyntaxKind.IdentifierDatabaseSymbol, oInnerJoin[0].ExpectedType);
            Assert.AreEqual("svc", ((Symbol)oInnerJoin[0]).Alias);

            Assert.AreEqual(SyntaxKind.IdentifierSubQuerySymbol, oInnerJoin[1].ExpectedType);
            Assert.AreEqual("omrbase", ((Symbol)oInnerJoin[1]).Alias);
        }

    }
}
