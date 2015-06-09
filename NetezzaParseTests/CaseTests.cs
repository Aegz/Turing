using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Lexer;
using Turing.Parser;
using Turing.Syntax;
using Turing.Syntax.Constructs.Symbols;

namespace NetezzaParseTests
{
    [TestClass]
    public class CaseTests
    {
        [TestMethod]
        public void ParseSimpleColumnCase()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT CASE WHEN ADDED ='INTERNATIONAL' THEN 'Y' ELSE 'N' END AS VARSTRING2
                        FROM FPC_SERVICE  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColList.ExpectedType);
            Assert.AreEqual(1, oColList.Count);

            SyntaxNode oCase = oColList[0];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oCase.ExpectedType);
            Assert.AreEqual(3, oCase.Count);
            Assert.AreEqual("VARSTRING2", ((Symbol)oCase).Alias);

            SyntaxNode oWhen = oCase[0];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen.ExpectedType);
            Assert.AreEqual(2, oWhen.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.EqualsToken, oWhen[0].ExpectedType);

            SyntaxNode oElse = oCase[1];
            Assert.AreEqual(SyntaxKind.ElseKeyword, oElse.ExpectedType);
            Assert.AreEqual(1, oElse.Count);
            Assert.AreEqual(SyntaxKind.LiteralToken, oElse[0].ExpectedType);
        }

        [TestMethod]
        public void ParseSimpleColumnCaseMissingEND()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT CASE WHEN ADDED ='INTERNATIONAL' THEN 'Y' ELSE 'N' AS VARSTRING2
                        FROM FPC_SERVICE  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColList.ExpectedType);
            Assert.AreEqual(1, oColList.Count);

            SyntaxNode oCase = oColList[0];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oCase.ExpectedType);
            Assert.AreEqual(3, oCase.Count);
            Assert.AreEqual("VARSTRING2", ((Symbol)oCase).Alias);

            SyntaxNode oWhen = oCase[0];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen.ExpectedType);
            Assert.AreEqual(2, oWhen.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.EqualsToken, oWhen[0].ExpectedType);

            SyntaxNode oElse = oCase[1];
            Assert.AreEqual(SyntaxKind.ElseKeyword, oElse.ExpectedType);
            Assert.AreEqual(1, oElse.Count);
            Assert.AreEqual(SyntaxKind.LiteralToken, oElse[0].ExpectedType);
        }


        [TestMethod]
        public void ParseComplexColumnCase()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT CASE     
                            WHEN ADDED ='INTERNATIONAL' THEN 'Y' 
                            WHEN svc.MKT_PROD IN ('MOB PT', 'MOB PP') THEN '-' 
                            ELSE 'N' END AS VARSTRING2
                        FROM FPC_SERVICE  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColList.ExpectedType);
            Assert.AreEqual(1, oColList.Count);

            SyntaxNode oCase = oColList[0];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oCase.ExpectedType);
            Assert.AreEqual(4, oCase.Count);
            Assert.AreEqual("VARSTRING2", ((Symbol)oCase).Alias);

            SyntaxNode oWhen = oCase[0];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen.ExpectedType);
            Assert.AreEqual(2, oWhen.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.EqualsToken, oWhen[0].ExpectedType);

            SyntaxNode oWhen2 = oCase[1];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen2.ExpectedType);
            Assert.AreEqual(2, oWhen2.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.InKeyword, oWhen2[0].ExpectedType);

            SyntaxNode oElse = oCase[2];
            Assert.AreEqual(SyntaxKind.ElseKeyword, oElse.ExpectedType);
            Assert.AreEqual(1, oElse.Count);
            Assert.AreEqual(SyntaxKind.LiteralToken, oElse[0].ExpectedType);
        }

        [TestMethod]
        public void ParseCascadedColumnCase()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        SELECT CASE     
                            WHEN ADDED ='INTERNATIONAL' THEN 
                                CASE WHEN svc.MKT_PROD IN ('MOB PT', 'MOB PP') THEN '-' ELSE '*' END
                            ELSE 'N' END AS VARSTRING2
                        FROM FPC_SERVICE  
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oSelect = oTemp.FindFirst(SyntaxKind.SelectKeyword);
            Assert.AreNotEqual(null, oSelect);
            Assert.AreEqual(2, oSelect.Count);

            SyntaxNode oColList = oSelect[0];
            Assert.AreEqual(SyntaxKind.ColumnListNode, oColList.ExpectedType);
            Assert.AreEqual(1, oColList.Count);

            SyntaxNode oCase = oColList[0];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oCase.ExpectedType);
            Assert.AreEqual(3, oCase.Count);
            Assert.AreEqual("VARSTRING2", ((Symbol)oCase).Alias);

            SyntaxNode oWhen = oCase[0];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen.ExpectedType);
            Assert.AreEqual(2, oWhen.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.EqualsToken, oWhen[0].ExpectedType);

            SyntaxNode oInnerCase = oWhen[1][0];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oInnerCase.ExpectedType);
            Assert.AreEqual(3, oInnerCase.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.InKeyword, oInnerCase[0][0].ExpectedType);

            SyntaxNode oElse = oCase[1];
            Assert.AreEqual(SyntaxKind.ElseKeyword, oElse.ExpectedType);
            Assert.AreEqual(1, oElse.Count);
            Assert.AreEqual(SyntaxKind.LiteralToken, oElse[0].ExpectedType);
        }


        [TestMethod]
        public void ParseSimpleWhereCase()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                    SELECT * FROM APAPP_CRMMP..MA504740005_COMM9729001
                    WHERE VARDATA3 = CASE WHEN VARDATA3 IS NOT NULL THEN 'MOBILE PHONE' ELSE '' END
                ");

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            SyntaxNode oWhere = oTemp.FindFirst(SyntaxKind.WhereKeyword);
            Assert.AreNotEqual(null, oWhere);
            Assert.AreEqual(1, oWhere.Count);

            SyntaxNode oEquals = oWhere[0];
            Assert.AreEqual(SyntaxKind.EqualsToken, oEquals.ExpectedType);
            Assert.AreEqual(2, oEquals.Count);

            SyntaxNode oCase = oEquals[1];
            Assert.AreEqual(SyntaxKind.CaseKeyword, oCase.ExpectedType);
            Assert.AreEqual(3, oCase.Count);

            SyntaxNode oWhen = oCase[0];
            Assert.AreEqual(SyntaxKind.WhenKeyword, oWhen.ExpectedType);
            Assert.AreEqual(2, oWhen.Count); // Literal and THEN
            Assert.AreEqual(SyntaxKind.IsKeyword, oWhen[0].ExpectedType);

            SyntaxNode oElse = oCase[1];
            Assert.AreEqual(SyntaxKind.ElseKeyword, oElse.ExpectedType);
            Assert.AreEqual(1, oElse.Count);
            Assert.AreEqual(SyntaxKind.LiteralToken, oElse[0].ExpectedType);
        }

    }
}
