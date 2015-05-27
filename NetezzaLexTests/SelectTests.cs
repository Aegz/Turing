using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaLexTests
{
    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void TestSelectStatementBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Should be 8 (including 1 for the EOF Token)
            Assert.IsTrue(oParser.TokenList.Count == 8);
            // First item should be a select keyword (ignoring all comments which is trivia)
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            // *
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.StarToken);
            // From
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.FromKeyword);

        }

        [TestMethod]
        public void TestIdentifierDatabase()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.IdentifierToken);
            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[3].RawSQLText, "APSHARE_FPVIEWS");
        }

        [TestMethod]
        public void TestIdentifierTable()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[5].ExpectedType, SyntaxKind.IdentifierToken);
            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[5].RawSQLText, "FPC_SERVICE");
        }

        [TestMethod]
        public void TestTableAlias()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[6].ExpectedType, SyntaxKind.IdentifierToken);
            // APSHARE_FPVIEWS - Identifier
            Assert.AreEqual(oParser.TokenList[6].RawSQLText, "svc");
        }

        [TestMethod]
        public void TestSelectStatementComplex()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc
                        WHERE svc.MKT_PROD_CD = 'A' ;            
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Should be 15 (including 1 for the semicolon)
            Assert.IsTrue(oParser.TokenList.Count == 15);
            // First item should be a select keyword (ignoring all comments which is trivia)
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
        }

        [TestMethod]
        public void TestINNERJOINStatementBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APMART_FPVIEWS..FPC_SERVICE svc
                        INNER JOIN 
                            APSHARE_FP..WR02290_FETCH ftch
                            ON svc.SVC_IDNTY = ftch.SERVICE_NO
                        WHERE svc.MKT_PROD_CD = 'A' ;            
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Should be 15 (including 1 for the semicolon)
            Assert.AreEqual(oParser.TokenList.Count, 29);
            // First item should be a select keyword (ignoring all comments which is trivia)
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);

            // We only really test for the JOIN and its positioning
            Assert.AreEqual(oParser.TokenList[7].ExpectedType, SyntaxKind.InnerJoinKeyword);
            Assert.AreEqual(oParser.TokenList[8].ExpectedType, SyntaxKind.JoinKeyword);
        }

        [TestMethod]
        public void TestWITHStatementBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        WITH _X AS
                        (
                            SELECT * FROM APMART_FPVIEWS..FPC_SERVICE LIMIT 100
                        )
                        SELECT  
                        *
                        FROM
                            APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            _X
                            ON _X.SVC_IDNTY = svc.SVC_IDNTY   
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // WITH
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.WithKeyword);
            Assert.AreEqual(oParser.TokenList[0].RawSQLText, "WITH");

            // Identifier
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[1].RawSQLText, "_X");

            // Brackets
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.OpenParenthesisToken);
            Assert.AreEqual(oParser.TokenList[3].RawSQLText, "(");
            Assert.AreEqual(oParser.TokenList[12].ExpectedType, SyntaxKind.CloseParenthesisToken);
            Assert.AreEqual(oParser.TokenList[12].RawSQLText, ")");
        }

        [TestMethod]
        public void TestWITHStatementComplex()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        WITH _X AS
                        (
                            SELECT * FROM APMART_FPVIEWS..FPC_SERVICE LIMIT 100
                        ),
                        _Y AS
                        (
                            SELECT * FROM APMART_FPVIEWS..FPC_ACCOUNT LIMIT 2
                        )
                        SELECT  
                        *
                        FROM
                            APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            _X
                            ON _X.SVC_IDNTY = svc.SVC_IDNTY   
                        WHERE _X.ACCT_ID IN (SELECT * FROM _Y)
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // WITH
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.WithKeyword);
            Assert.AreEqual(oParser.TokenList[0].RawSQLText, "WITH");

            // Identifier
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[1].RawSQLText, "_X");

            // Brackets
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.OpenParenthesisToken);
            Assert.AreEqual(oParser.TokenList[3].RawSQLText, "(");
            Assert.AreEqual(oParser.TokenList[12].ExpectedType, SyntaxKind.CloseParenthesisToken);
            Assert.AreEqual(oParser.TokenList[12].RawSQLText, ")");

            // Comma
            Assert.AreEqual(oParser.TokenList[13].ExpectedType, SyntaxKind.CommaToken);
            Assert.AreEqual(oParser.TokenList[13].RawSQLText, ",");
            // Second set of brackets
            Assert.AreEqual(oParser.TokenList[16].ExpectedType, SyntaxKind.OpenParenthesisToken);
            Assert.AreEqual(oParser.TokenList[16].RawSQLText, "(");
        }

        [TestMethod]
        public void TestSelectExpression()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        COUNT(*)
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc     
                             
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Should be 8 (including 1 for the EOF Token)
            Assert.AreEqual(11, oParser.TokenList.Count);
            // First item should be a select keyword (ignoring all comments which is trivia)
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            // COUNT
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.CountKeyword);
            // (
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.OpenParenthesisToken);
            // (
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.StarToken);
            // )
            Assert.AreEqual(oParser.TokenList[4].ExpectedType, SyntaxKind.CloseParenthesisToken);
        }


    }
}
