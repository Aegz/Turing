using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaLexTests
{
    [TestClass]
    public class ArithmaticTests
    {
        #region Minus
        [TestMethod]
        public void LexMinusInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty - 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.MinusToken);
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.NumericToken);

        }

        [TestMethod]
        public void LexMinusInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        WHERE svc_idnty - 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[8].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[9].ExpectedType, SyntaxKind.MinusToken);
            Assert.AreEqual(oParser.TokenList[10].ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void LexMinusInON()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            APSHARE_FP..WR02052 omr
                            ON svc.SVC_IDNTY = omr.SERViCE_NO AND
                            svc.sERVICE_NO - 123123 = 0
                        WHERE svc_idnty - 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[24].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[25].ExpectedType, SyntaxKind.MinusToken);
            Assert.AreEqual(oParser.TokenList[26].ExpectedType, SyntaxKind.NumericToken);
        }
        #endregion

        #region Slash
        [TestMethod]
        public void LexSlashInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty / 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);


            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.SlashToken);
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.NumericToken);

        }

        [TestMethod]
        public void LexSlashInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        WHERE svc_idnty / 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[8].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[9].ExpectedType, SyntaxKind.SlashToken);
            Assert.AreEqual(oParser.TokenList[10].ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void LexSlashInON()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            APSHARE_FP..WR02052 omr
                            ON svc.SVC_IDNTY = omr.SERViCE_NO AND
                            svc.sERVICE_NO / 123123 = 0
                        WHERE svc_idnty - 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[24].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[25].ExpectedType, SyntaxKind.SlashToken);
            Assert.AreEqual(oParser.TokenList[26].ExpectedType, SyntaxKind.NumericToken);
        }
        #endregion

        #region Plus
        [TestMethod]
        public void LexPlusInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty + 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.PlusToken);
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void LexPlusInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        WHERE svc_idnty + 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[8].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[9].ExpectedType, SyntaxKind.PlusToken);
            Assert.AreEqual(oParser.TokenList[10].ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void LexPlusInOn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            APSHARE_FP..WR02052 omr
                            ON svc.SVC_IDNTY = omr.SERViCE_NO AND
                            svc.sERVICE_NO + 123123 = 0
                        WHERE svc_idnty - 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[24].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[25].ExpectedType, SyntaxKind.PlusToken);
            Assert.AreEqual(oParser.TokenList[26].ExpectedType, SyntaxKind.NumericToken);
        }
        #endregion

        #region Star
        [TestMethod]
        public void LexStarInColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty * 2
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);


            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.SelectKeyword);
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.StarToken);
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.NumericToken);

        }

        [TestMethod]
        public void LexStarInWhere()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        WHERE svc_idnty * 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[8].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[9].ExpectedType, SyntaxKind.StarToken);
            Assert.AreEqual(oParser.TokenList[10].ExpectedType, SyntaxKind.NumericToken);
        }

        [TestMethod]
        public void LexStarInOn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* TEST */      
                        SELECT  
                        svc_idnty
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc   
                        INNER JOIN
                            APSHARE_FP..WR02052 omr
                            ON svc.SVC_IDNTY = omr.SERViCE_NO AND
                            svc.sERVICE_NO * 123123 = 0
                        WHERE svc_idnty - 2 = 4       
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            Assert.AreEqual(oParser.TokenList[24].ExpectedType, SyntaxKind.IdentifierToken);
            Assert.AreEqual(oParser.TokenList[25].ExpectedType, SyntaxKind.StarToken);
            Assert.AreEqual(oParser.TokenList[26].ExpectedType, SyntaxKind.NumericToken);
        }
        #endregion

    }
}
