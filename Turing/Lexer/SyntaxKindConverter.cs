using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax;

namespace Turing.Lexer
{
    class SyntaxKindConverter
    {
        /// <summary>
        /// Converts straight text into the appropriate SyntaxKind Keyword
        /// </summary>
        /// <param name="xsGivenText"></param>
        /// <returns></returns>
        public static SyntaxKind ConvertKeywordIntoSyntaxKind(String xsGivenText)
        {
            // Make sure its clean (otherwise this is a waste of time)
            String sCleanText = xsGivenText.Trim().ToUpper();

            switch (sCleanText)
            {
                // SAS Specific
                case "EXTRACT":
                    return SyntaxKind.ExecuteKeyword;

                // CORE
                case "SEL":
                case "SELECT":
                    return SyntaxKind.SelectKeyword;
                case "INSERT":
                    return SyntaxKind.InsertKeyword;
                case "UPDATE":
                    return SyntaxKind.UpdateKeyword;
                case "DELETE":
                    return SyntaxKind.DeleteKeyword;
                case "ALTER":
                    return SyntaxKind.AlterKeyword;
                case "WITH":
                    return SyntaxKind.WithKeyword;
                case "CREATE":
                    return SyntaxKind.CreateKeyword;


                // Attributes

                // CREATE
                case "VIEW":
                    return SyntaxKind.ViewKeyword;
                case "TABLE":
                    return SyntaxKind.TableKeyword;
                case "DISTRIBUTE":
                    return SyntaxKind.DistributeKeyword;
                case "TEMP":
                case "TEMPORARY":
                    return SyntaxKind.TempKeyword;

                // SELECT
                case "FROM":
                    return SyntaxKind.FromKeyword;
                case "WHERE":
                    return SyntaxKind.WhereKeyword;
                case "GROUP BY":
                    return SyntaxKind.GroupByKeyword;
                case "ORDER BY":
                    return SyntaxKind.OrderByKeyword;
                case "LIMIT":
                    return SyntaxKind.LimitKeyword;

                // INSERT
                case "INTO":
                    return SyntaxKind.InsertKeyword;

                // UPDATE
                case "SET":
                    return SyntaxKind.SetKeyword;

                // JOIN
                case "INNER":
                    return SyntaxKind.InnerJoinKeyword;
                case "OUTER":
                    return SyntaxKind.OuterJoinKeyword;
                case "LEFT":
                    return SyntaxKind.LeftJoinKeyword;
                case "RIGHT":
                    return SyntaxKind.RightJoinKeyword;
                case "CROSS":
                    return SyntaxKind.CrossJoinKeyword;
                case "JOIN":
                    return SyntaxKind.JoinKeyword;
                case "ON":
                    return SyntaxKind.OnKeyword;

                // Conditional
                case "IN":
                    return SyntaxKind.InKeyword;
                case "NOT":
                    return SyntaxKind.NotKeyword;
                case "IS":
                    return SyntaxKind.IsKeyword;
                case "NULL":
                    return SyntaxKind.NullKeyword;
                case "AND":
                    return SyntaxKind.AndKeyword;
                case "OR":
                    return SyntaxKind.OrKeyword;

                // CASE Specific
                case "CASE":
                    return SyntaxKind.CaseKeyword;
                case "WHEN":
                    return SyntaxKind.WhenKeyword;
                case "THEN":
                    return SyntaxKind.ThenKeyword;
                case "ELSE":
                    return SyntaxKind.ElseKeyword;
                case "END":
                    return SyntaxKind.EndKeyword;

                // Other
                case "AS":
                    return SyntaxKind.AsKeyword;

                // True/False
                case "TRUE":
                case "FALSE":
                    return SyntaxKind.BooleanToken;


                default:
                    return SyntaxKind.UnknownToken;
            }
        }
    }
}
