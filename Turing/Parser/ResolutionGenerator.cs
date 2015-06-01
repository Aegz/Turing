using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Factories;
using Turing.Syntax;

namespace Turing.Parser
{
    /// <summary>
    /// This class attempts to resolve possible issues
    /// using business rules that are defined below
    /// </summary>
    class ResolutionGenerator
    {
        public static void BinaryExpression(SyntaxNode xoCurrentNode)
        {
            // If nothing was added
            if (xoCurrentNode.Children.Count == 0)
            {
                // Just add an identifier saying that we are missing the item
                xoCurrentNode.AddChild(
                    SyntaxNodeFactory.FactoryCreateMissingNode(
                        SyntaxKindUtilities.GetStringFromKind(SyntaxKind.IdentifierToken)));
                //xoCurrentNode.AddChild(new SyntaxNode(new SyntaxToken(SyntaxKind.IdentifierToken, "MISSING IDN/Literal/Expression"), NULL_STRATEGY));
                xoCurrentNode.Comments.Add(new Diagnostics.StatusItem("No LEFT operator found"));
            }
        }

        public static void Join(SyntaxNode xoCurrentNode)
        {
            // If nothing was added
            if (xoCurrentNode.Children.Count == 0)
            {
                // Just add an identifier saying that we are missing the item
                xoCurrentNode.AddChild(
                    SyntaxNodeFactory.FactoryCreateMissingNode(
                        SyntaxKindUtilities.GetStringFromKind(SyntaxKind.IdentifierToken)));

                //xoCurrentNode.AddChild(new SyntaxNode(new SyntaxToken(SyntaxKind.IdentifierToken, "MISSING IDN/Literal/Expression"), NULL_STRATEGY));
                xoCurrentNode.Comments.Add(new Diagnostics.StatusItem("No LEFT table found"));
            }
        }


        public static void Parenthesis(SyntaxNode xoCurrentNode)
        {
            // If nothing was added
            if (xoCurrentNode.Children.Count == 0)
            {
                xoCurrentNode.AddChild(SyntaxNodeFactory.FactoryCreateMissingNode(SyntaxKindUtilities.GetStringFromKind(SyntaxKind.IdentifierToken)));
            }
        }

    }
}
