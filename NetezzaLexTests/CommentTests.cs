using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaLexTests
{
    [TestClass]
    public class CommentTests
    {
        [TestMethod]
        public void LexMultiLineBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        /* 
                            Line 1
                            Line 2
                        */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.MultiLineCommentTrivia, oParser.TokenList[0].LeadingTrivia[3].ExpectedType);
        }

        [TestMethod]
        public void LexMultiLineInlineColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        SELECT  
                        * /* Star, for everything */
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.WhitespaceTrivia, oParser.TokenList[1].TrailingTrivia[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.MultiLineCommentTrivia, oParser.TokenList[1].TrailingTrivia[1].ExpectedType);
        }

        [TestMethod]
        public void LexMultiLineInlineTable()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        SELECT  
                        * 
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc /* Star, for everything */      
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.WhitespaceTrivia, oParser.TokenList[6].TrailingTrivia[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.MultiLineCommentTrivia, oParser.TokenList[6].TrailingTrivia[1].ExpectedType);
        }


        [TestMethod]
        public void LexSingleLineBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        -- Basic Select Statement
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.SingleLineCommentTrivia, oParser.TokenList[0].LeadingTrivia[3].ExpectedType);
        }

        [TestMethod]
        public void LexSingleLineInlineColumn()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        
                        SELECT  
                        * -- Basic Select Statement
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.WhitespaceTrivia, oParser.TokenList[1].TrailingTrivia[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.SingleLineCommentTrivia, oParser.TokenList[1].TrailingTrivia[1].ExpectedType);
        }

        [TestMethod]
        public void LexSingleLineInlineTable()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
            @"   
                        
                        SELECT  
                        * 
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc -- Basic Select Statement          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Check the trivia type
            Assert.AreEqual(SyntaxKind.WhitespaceTrivia, oParser.TokenList[6].TrailingTrivia[0].ExpectedType);
            Assert.AreEqual(SyntaxKind.SingleLineCommentTrivia, oParser.TokenList[6].TrailingTrivia[1].ExpectedType);
        }


    }
}
