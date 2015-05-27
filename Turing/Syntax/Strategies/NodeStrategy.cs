using System;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;


namespace Turing.Syntax.Strategies
{
    public class NodeStrategy
    {
        // To String (Just have the transformation stage handle this)
        // Can Consume Previous
        // TryConsume Previous


        public Func<SyntaxNode, SyntaxTokenList, Boolean> ConsumptionFn;                // Can eat node
        public Func<SyntaxNode, SyntaxTokenList, SyntaxNode> ConvertTokenFn;            // Convert that node

        public Func<SyntaxNode, SyntaxTokenList, Boolean> PreProcessFn;                 // Can preprocess
        public Func<SyntaxNode, SyntaxNode, SyntaxTokenList, Boolean> PostProcessFn;    // Add Etc

        public NodeStrategy()
        {
        }

        public NodeStrategy(
                Func<SyntaxNode, SyntaxTokenList, Boolean> xoConsumptionFn,
                Func<SyntaxNode, SyntaxTokenList, Boolean> xoPreProcessFn,
                Func<SyntaxNode, SyntaxTokenList, SyntaxNode> xoConvertTokenFn,
                Func<SyntaxNode, SyntaxNode, SyntaxTokenList, Boolean> xoPostProcessFn
            )
        {
            ConsumptionFn = xoConsumptionFn;
            PreProcessFn = xoPreProcessFn;
            ConvertTokenFn = xoConvertTokenFn;
            PostProcessFn = xoPostProcessFn;
        }

    }
}
