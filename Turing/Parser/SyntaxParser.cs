using Turing.Lexer;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Factories;

namespace Turing.Parser
{
    public class SyntaxParser
    {
        #region Object Attributes

        protected SyntaxLexer oLexer;       
        public SyntaxTokenList TokenList;

        #endregion


        public SyntaxParser (SlidingTextWindow TextWindow)
        {
            TokenList = new SyntaxTokenList();
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
        }

        /// <summary>
        /// Core function for this object. It will attempt to build a source 
        /// program tree from the stored list of tokens
        /// </summary>
        /// <returns></returns>
        public SyntaxNode ParseTree()
        {
            // If there is nothing to process, break early
            if (TokenList.HasTokensLeftToProcess())
            {
                // Pull off the first node
                SyntaxNode CurrentNode = SyntaxNodeFactory.ContextSensitiveConvertTokenToNode(TokenList.PopToken(), null);

                // If we consumed anything
                if (CurrentNode.TryConsumeList(TokenList))
                {
                    // Return the node
                    return CurrentNode;
                }

            }

            // Default to exception
            return null;
        }

    }
}
