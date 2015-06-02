using System;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Parser;
using Turing.Syntax.Collections;


namespace Turing.Syntax.Strategies
{

    /// <summary>
    /// Enumerated type to encapsulate possible results
    /// from consumption of a previous node
    /// or next node
    /// </summary>
    public enum CanConsumeResult
    {
        Complete = 1,   // Cannot process anymore
        Consume  = 2,   // Can consume the node
        Skip     = 3,   // Skip the next node
    }


    public class NodeStrategy
    {
        // To String (Just have the transformation stage handle this)
        // TryConsume Previous

        public Func<ParsingContext, CanConsumeResult> EligibilityFn;
        public Func<ParsingContext, SyntaxNode> TryConsumeNextFn;     // Consume Next Sibling
        public Func<ParsingContext, Boolean> PostProcessFn;    // Add Etc

        public NodeStrategy()
        {
        }

        public NodeStrategy(
                Func<ParsingContext, CanConsumeResult> xoEligibilityFn,
                Func<ParsingContext, SyntaxNode> xoConsumeNext,
                Func<ParsingContext, Boolean> xoPostProcessFn
            )
        {
            TryConsumeNextFn = xoConsumeNext;
            EligibilityFn = xoEligibilityFn;
            PostProcessFn = xoPostProcessFn;
        }

    }
}
