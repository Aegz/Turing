using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax
{
    class SyntaxKindFacts
    {


        public static Boolean IsJoinKeyword(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.JoinKeyword ||
                xeKind == SyntaxKind.InnerJoinKeyword ||
                xeKind == SyntaxKind.OuterKeyword ||
                xeKind == SyntaxKind.CrossJoinKeyword ||
                xeKind == SyntaxKind.LeftJoinKeyword ||
                xeKind == SyntaxKind.RightJoinKeyword;
        }

        public static Boolean IsUnaryOperator(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.NotKeyword ||
                xeKind == SyntaxKind.IsKeyword;
        }

        public static Boolean IsAdjunctConditionalOperator(SyntaxKind xeKind)
        {
            return
            xeKind == SyntaxKind.AndKeyword ||
            xeKind == SyntaxKind.OrKeyword;
        }

        public static Boolean IsConditionalOperator(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.InKeyword ||
                xeKind == SyntaxKind.IsKeyword ||
                xeKind == SyntaxKind.LikeKeyword ||

                xeKind == SyntaxKind.EqualsToken ||
                xeKind == SyntaxKind.GreaterThanOrEqualToken ||
                xeKind == SyntaxKind.GreaterThanToken ||
                xeKind == SyntaxKind.LessThanOrEqualToToken ||
                xeKind == SyntaxKind.LessThanToken ||
                xeKind == SyntaxKind.DiamondToken;
        }

        public static Boolean IsArithmaticOperator(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.PlusToken ||
                xeKind == SyntaxKind.MinusToken ||
                xeKind == SyntaxKind.StarToken ||
                xeKind == SyntaxKind.SlashToken;
        }

        public static Boolean IsIdentifier(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.CaseKeyword || // CASE can substitute Identifier
                xeKind == SyntaxKind.OpenParenthesisToken || // Could always be a (Idn)
                xeKind == SyntaxKind.IdentifierToken;
        }

        public static Boolean IsExpression(SyntaxKind xeKind)
        {
            return
                // Literals are types of expressions
                xeKind == SyntaxKind.NumericToken ||
                xeKind == SyntaxKind.LiteralToken ||
                xeKind == SyntaxKind.BooleanToken ||
                ((int)xeKind >= (int)SyntaxKind.AverageKeyword &&
                (int)xeKind <= (int)SyntaxKind.UpperKeyword);
        }

        public static Boolean IsIdentifierOrExpression(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.CaseKeyword ||
                IsIdentifier(xeKind) ||
                IsExpression(xeKind);
        }
    }
}
