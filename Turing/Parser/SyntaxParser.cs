using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Lexer;
using Turing.Syntax;
using Turing.Syntax.Collections;
using Turing.Syntax.Constructs;
using Turing.Syntax.Constructs.Keywords;

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


        public SyntaxNode ParseTree()
        {

            // If there is nothing to process, break early
            if (TokenList.HasTokensLeftToProcess())
            {
                // Pull off the first node
                SyntaxNode CurrentNode = SyntaxNodeFactory.NonContextSensitiveConvertTokenToNode(TokenList.PopToken());

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
