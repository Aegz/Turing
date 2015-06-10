using Turing.Lexer;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Factories;
using Turing.Syntax.Constructs;

namespace Turing.Parser
{
    public class SyntaxParser
    {
        #region Object Attributes

        protected SyntaxLexer oLexer;       
        public SyntaxList TokenList;

        #endregion


        public SyntaxParser (SlidingTextWindow TextWindow)
        {
            TokenList = new SyntaxList();
            oLexer = new SyntaxLexer(TextWindow);

            // Must initialise after object variables initialised
            this.Initialise();
        }

        /// <summary>
        /// Container method for any cumbersome initialisation methods
        /// </summary>
        private void Initialise()
        {
            // Generate a list of tokens from the available text
            while (oLexer.HasTokensLeftToProcess())
            {
                // Add any tokens found into our list
                TokenList.Add(oLexer.LexNextToken());
            }

            // make sure we have an EOF at the end
            if (TokenList[TokenList.Count - 1].ExpectedType != SyntaxKind.EOFNode)
            {
                TokenList.Add(new SyntaxToken(SyntaxKind.EOFNode, ""));
            }
        }

        /// <summary>
        /// Core function for this object. It will attempt to build a source 
        /// program tree from the stored list of tokens
        /// </summary>
        /// <returns></returns>
        public SyntaxNode ParseTree()
        {
            // Initialise the Query
            QuerySyntaxNode oQuery = new QuerySyntaxNode();

            // WHILE has tokens to process
            // Anytime we get here, create a new statement?
            while (TokenList.HasTokensLeftToProcess())
            {
                // Initialise a new statement every time we get here
                StatementSyntaxNode oStatement = new StatementSyntaxNode();

                // Pull off the first node
                SyntaxNode CurrentNode = SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(new ParsingContext(oQuery, TokenList));

                // If we consumed anything
                if (CurrentNode.TryConsumeList(new ParsingContext(CurrentNode, TokenList)))
                {

                }

                // Append to the statement anyways
                oStatement.Add(CurrentNode);

                oQuery.Add(oStatement);
            }

            // Return the Query
            return oQuery;
 
        }

    }
}
