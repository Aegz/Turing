using System;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax.Collections;


namespace Turing.Syntax.Strategies
{
    public class NodeStrategy
    {
        public Func<SyntaxNode, SyntaxTokenList, Boolean> ConsumptionFn;
        public Func<SyntaxNode, SyntaxTokenList, Boolean> PreProcessFn;
        public Func<SyntaxNode, SyntaxTokenList, SyntaxNode> ConvertTokenFn;
        public Func<SyntaxNode, SyntaxNode, SyntaxTokenList, Boolean> PostProcessFn;

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
