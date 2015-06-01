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

                #region Statements
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

                #endregion

                // Attributes
                #region Attributes
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
                #endregion

                #region Join
                // JOIN
                case "INNER":
                    return SyntaxKind.InnerJoinKeyword;
                case "OUTER":
                    return SyntaxKind.OuterKeyword;
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
                #endregion

                #region Conditional Operators
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

                #endregion

                #region CASE
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
                #endregion

                // Other
                case "AS":
                    return SyntaxKind.AsKeyword;
                case "ALL":
                    return SyntaxKind.AllKeyword;
                case "DISTINCT":
                    return SyntaxKind.DistinctKeyword;
                case "UNIQUE":
                    return SyntaxKind.UniqueKeyword;
                case "ASC":
                    return SyntaxKind.ASCKeyword;
                case "DESC":
                    return SyntaxKind.DESCKeyword;

                // True/False
                case "TRUE":
                case "FALSE":
                    return SyntaxKind.BooleanToken;

                // First/Last
                case "FIRST":
                    return SyntaxKind.FirstKeyword;
                case "LAST":
                    return SyntaxKind.LastKeyword;



                #region Functions

                case "AVG":
                case "AVERAGE":
                    return SyntaxKind.AverageKeyword;
                case "COUNT":
                    return SyntaxKind.CountKeyword;
                case "MAX":
                    return SyntaxKind.MaxKeyword;
                case "SUM":
                    return SyntaxKind.SumKeyword;
                


                case "COALESCE":
                    return SyntaxKind.CoalesceKeyword;
                case "POSITION":
                    return SyntaxKind.PositionKeyword;
                case "PARTITION":
                    return SyntaxKind.PartitionKeyword;
                case "RANK":
                    return SyntaxKind.RankKeyword;
                case "ROW_NUMBER":
                    return SyntaxKind.RowNumberKeyword;
                case "SUBSTRING":
                    return SyntaxKind.SubStringKeyword;
                case "TRIM":
                    return SyntaxKind.TrimKeyword;
                case "LEADING":
                    return SyntaxKind.LeadingKeyword;
                case "TRAILING":
                    return SyntaxKind.TrailingKeyword;

                case "LOWER":
                    return SyntaxKind.LowerKeyword;
                case "UPPER":
                    return SyntaxKind.UpperKeyword;

                // DATE
                case "DATE_PART":
                    return SyntaxKind.DatePartKeyword;
                case "DATE_TRUNC":
                    return SyntaxKind.DateTruncKeyword;
                case "TIMEOFDAY":
                    return SyntaxKind.TimeOfDayKeyword;
                case "TIMESTAMP":
                    return SyntaxKind.DateTruncKeyword;

                // Conversion
                case "CAST":
                    return SyntaxKind.CastKeyword;
                case "TO_CHAR":
                    return SyntaxKind.ToCharKeyword;
                case "TO_DATE":
                    return SyntaxKind.ToDateKeyword;


                #endregion

                default:
                    return SyntaxKind.UnknownNode;
            }
        }


        
    }
}
