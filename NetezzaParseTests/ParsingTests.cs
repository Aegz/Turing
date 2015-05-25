using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Turing.Syntax;
using Turing.Parser;
using System.Collections.Generic;
using Turing.Lexer;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Keywords;
using Turing.Syntax.Constructs.Symbols.Collections;

namespace NetezzaParseTests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        public void TestSelectTree()
        {
            SlidingTextWindow oText = new SlidingTextWindow(
                @"   
                        /* TEST */      
                        SELECT  
                        *
                        FROM
                        APSHARE_FPVIEWS..FPC_SERVICE svc          
                ");

            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.IsTrue(oTemp.GetType() == typeof(QuerySyntaxNode));

            // Make sure we have only statements underneath
            foreach (SyntaxNode oStatement in oTemp.Children)
            {
                Assert.IsTrue(oStatement.GetType() == typeof(StatementSyntaxNode));

                // All core keywords here are SELECT
                foreach (SyntaxNode oCoreKeyword in oStatement.Children)
                {
                    Assert.IsTrue(oCoreKeyword.GetType() == typeof(SelectSyntaxNode));
                    // All core keywords here are FROM
                    foreach (SyntaxNode oFromKeyword in oCoreKeyword.Children)
                    {
                        if (oFromKeyword.GetType() == typeof(FromSyntaxNode))
                        {
                            // Should be an identifier
                            foreach (SyntaxNode oTable in oFromKeyword.Children)
                            {
                                Assert.IsTrue(oTable.ExpectedType == SyntaxKind.IdentifierToken);
                            }
                        }
                        else
                        {
                            Assert.IsTrue(oFromKeyword.GetType() == typeof(SymbolList));
                        }
                    }
                }
            }
        }

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
                        )      
                ");

            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.IsTrue(oTemp.GetType() == typeof(QuerySyntaxNode));

            // Make sure we have only statements underneath
            foreach (SyntaxNode oStatement in oTemp.Children)
            {
                Assert.IsTrue(oStatement.GetType() == typeof(StatementSyntaxNode));

                // All core keywords here are SELECT
                foreach (SyntaxNode oCoreKeyword in oStatement.Children)
                {
                    Assert.IsTrue(oCoreKeyword.GetType() == typeof(SelectSyntaxNode));
                    // All core keywords here are FROM
                    foreach (SyntaxNode oFromKeyword in oCoreKeyword.Children)
                    {
                        if (oFromKeyword.GetType() == typeof(FromSyntaxNode))
                        {
                            // Should be an identifier
                            foreach (SyntaxNode oTable in oFromKeyword.Children)
                            {
                                Assert.IsTrue(oTable.ExpectedType == SyntaxKind.IdentifierToken);
                            }
                        }
                        else
                        {
                            Assert.IsTrue(oFromKeyword.GetType() == typeof(SymbolList));
                        }
                    }
                }
            }
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

            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.IsTrue(oTemp.GetType() == typeof(QuerySyntaxNode));

            // Make sure we have only statements underneath
            foreach (SyntaxNode oStatement in oTemp.Children)
            {
                // All core keywords here are SELECT
                foreach (SyntaxNode oCoreKeyword in oStatement.Children)
                {
                    // All core keywords here are FROM
                    foreach (SyntaxNode oFromKeyword in oCoreKeyword.Children)
                    {
                        if (oFromKeyword.GetType() == typeof(FromSyntaxNode))
                        {
                            // Should be a JOIN
                            foreach (SyntaxNode oTable in oFromKeyword.Children)
                            {
                                Assert.IsTrue(oTable.GetType() == typeof(JoinSyntaxNode));
                            }
                        }
                        else
                        {
                            Assert.IsTrue(oFromKeyword.GetType() == typeof(SymbolList));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestComplexJoinTree()
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

            // A flat list which will be used by the parser to generate an actual tree
            List<SyntaxToken> aoList = new List<SyntaxToken>();

            // Initialises the Parser
            SyntaxParser oParser = new SyntaxParser(oText);

            // Try and generate a tree
            SyntaxNode oTemp = oParser.ParseTree();

            // Make sure we have a Query
            Assert.IsTrue(oTemp.GetType() == typeof(QuerySyntaxNode));

            // Make sure we have only statements underneath
            foreach (SyntaxNode oStatement in oTemp.Children)
            {
                // All core keywords here are SELECT
                foreach (SyntaxNode oCoreKeyword in oStatement.Children)
                {
                    // All core keywords here are FROM
                    foreach (SyntaxNode oFromKeyword in oCoreKeyword.Children)
                    {
                        if (oFromKeyword.GetType() == typeof(FromSyntaxNode))
                        {
                            // Should be a JOIN
                            foreach (SyntaxNode oTable in oFromKeyword.Children)
                            {
                                Assert.IsTrue(oTable.GetType() == typeof(JoinSyntaxNode));
                            }
                        }
                        else
                        {
                            Assert.IsTrue(oFromKeyword.GetType() == typeof(SymbolList));
                        }
                    }
                }
            }
        }

    }
}
