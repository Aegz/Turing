using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax
{
    class SyntaxKindFacts
    {
        #region Keyword Specific

        public static Boolean IsKeyword(SyntaxKind xeKind)
        {
            return ((int)xeKind >= (int)SyntaxKind.FromKeyword &&
                (int)xeKind <= (int)SyntaxKind.NewKeyword);
        }

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

        #endregion

        public static Boolean IsTerminatingNode(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.EOFNode ||
                xeKind == SyntaxKind.EOFTrivia ||
                xeKind == SyntaxKind.SemiColonToken;
        }

        #region Operators

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
                xeKind == SyntaxKind.NotLikeKeyword ||
                xeKind == SyntaxKind.LikeKeyword ||
                xeKind == SyntaxKind.NotInKeyword ||
                xeKind == SyntaxKind.IsKeyword ||

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

        public static Boolean IsBinaryConstruct(SyntaxKind xeKind)
        {
            return
                IsArithmaticOperator(xeKind) ||
                IsConditionalOperator(xeKind) ||
                IsJoinKeyword(xeKind) ||
                IsAdjunctConditionalOperator(xeKind);
        }

        #endregion

        #region Identifiers and Expressions

        public static Boolean IsIdentifier(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.CaseKeyword || // CASE can substitute Identifier
                xeKind == SyntaxKind.OpenParenthesisToken || // Could always be a (Idn)
                xeKind == SyntaxKind.IdentifierToken ||
                // More specific Identifiers
                xeKind == SyntaxKind.IdentifierDatabaseSymbol ||
                xeKind == SyntaxKind.IdentifierSchemaSymbol ||
                xeKind == SyntaxKind.IdentifierTableSymbol ||
                xeKind == SyntaxKind.IdentifierSubQuerySymbol ||
                xeKind == SyntaxKind.IdentifierColumnSymbol;
        }

        public static Boolean IsExpression(SyntaxKind xeKind)
        {
            return
                // Literals are types of expressions
                IsLiteral(xeKind) ||
                IsFunction(xeKind);
        }

        public static Boolean IsLiteral(SyntaxKind xeKind)
        {
            return 
                xeKind == SyntaxKind.NullKeyword || 
                xeKind == SyntaxKind.NumericToken ||
                xeKind == SyntaxKind.LiteralToken ||
                xeKind == SyntaxKind.BooleanToken;
        }

        public static Boolean IsFunction(SyntaxKind xeKind)
        {
            return ((int)xeKind >= (int)SyntaxKind.AverageKeyword &&
                (int)xeKind <= (int)SyntaxKind.UpperKeyword);
        }

        public static Boolean IsIdentifierOrExpression(SyntaxKind xeKind)
        {
            return
                xeKind == SyntaxKind.CaseKeyword ||
                IsIdentifier(xeKind) ||
                IsExpression(xeKind);
        }

        #endregion
    }
}
