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

        // Used for generation of the tree
        private List<SyntaxNode> Statements;

        public SyntaxNode ParseTree()
        {
            // Initialise a list of statements
            Statements = new List<SyntaxNode>();

            SyntaxNode CurrentNode;
            // Use a while loop for more control
            while (TokenList.HasTokensLeftToProcess())
            {
                // Get the next token
                CurrentNode = TokenList.PopToken();

                // Convert the Token into a proper Node if necessary

                // Have the node Consume from the list

                // Rinse and repeat
            }

            return new SyntaxNode(SyntaxKind.UnknownToken, "?");
        }

    }
}
