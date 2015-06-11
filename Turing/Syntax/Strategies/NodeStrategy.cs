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
        Unknown  = 4,   // Unexpected item
    }


    public class NodeStrategy
    {
        // To String (Just have the transformation stage handle this)
        // TryConsume Previous

        public Func<ParsingContext, Boolean, CanConsumeResult> EligibilityFn;
        public Func<ParsingContext, Boolean, SyntaxNode> TryConsumeNextFn;     // Consume Next Sibling
        public Func<ParsingContext, Boolean> ValidationFn;    

        public NodeStrategy()
        {
        }

        public NodeStrategy(
                Func<ParsingContext, Boolean, CanConsumeResult> xoEligibilityFn,
                Func<ParsingContext, Boolean, SyntaxNode> xoConsumeNextFn,
                Func<ParsingContext, Boolean> xoValidationFn
            )
        {
            TryConsumeNextFn = xoConsumeNextFn;
            EligibilityFn = xoEligibilityFn;
            ValidationFn = xoValidationFn;
        }

    }
}
