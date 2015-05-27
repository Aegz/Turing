using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;

namespace NetezzaLexTests
{
    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        public void CreateTableBasic()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        CREATE TABLE _X AS 
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            SyntaxParser oParser = new SyntaxParser(oText);

            // Should be 8 (including 1 for the EOF Token)
            Assert.IsTrue(oParser.TokenList.Count == 12);

            // First item should be a CREATE keyword (ignoring all comments which is trivia)
            Assert.AreEqual(oParser.TokenList[0].ExpectedType, SyntaxKind.CreateKeyword);
            // _X
            Assert.AreEqual(oParser.TokenList[1].ExpectedType, SyntaxKind.TableKeyword);
            // Iden
            Assert.AreEqual(oParser.TokenList[2].ExpectedType, SyntaxKind.IdentifierToken);
            // Iden
            Assert.AreEqual(oParser.TokenList[3].ExpectedType, SyntaxKind.AsKeyword);
        }
    }
}
