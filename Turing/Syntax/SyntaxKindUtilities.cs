using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Syntax
{
    /// <summary>
    /// Contains the SyntaxKind -> String conversion and vice versa
    /// 
    /// This is preferred method over using the case statement switch to convert a
    /// SyntaxKind into a string and the reverse operation since it is
    /// significantly easier to maintain
    /// </summary>
    class SyntaxKindUtilities
    {
        #region Core Syntax Conversions
        // DO NOT mess with the ordering here
        private static Dictionary<String, SyntaxKind> Conversions = new Dictionary<string, SyntaxKind>()
        {
            // SAS Specific
            { "EXECUTE",  SyntaxKind.ExecuteKeyword},

            #region Statements
            
            { "SELECT",  SyntaxKind.SelectKeyword},
            { "SEL",  SyntaxKind.SelectKeyword},
            { "INSERT",  SyntaxKind.InsertKeyword},
            { "UPDATE",  SyntaxKind.UpdateKeyword},
            { "DELETE",  SyntaxKind.DeleteKeyword},
            { "ALTER",  SyntaxKind.AlterKeyword},
            { "WITH",  SyntaxKind.WithKeyword},
            { "CREATE",  SyntaxKind.CreateKeyword},

            #endregion

            #region Attributes
            // CREATE
            { "VIEW",  SyntaxKind.ViewKeyword},
            { "TABLE",  SyntaxKind.TableKeyword},
            { "DISTRIBUTE",  SyntaxKind.DistributeKeyword},
            { "TEMP",  SyntaxKind.TempKeyword},
            { "TEMPORARY",  SyntaxKind.TempKeyword},

            // SELECT
            { "FROM",  SyntaxKind.FromKeyword},
            { "WHERE",  SyntaxKind.WhereKeyword},
            { "GROUP BY",  SyntaxKind.GroupByKeyword},
            { "ORDER BY",  SyntaxKind.OrderByKeyword},
            { "LIMIT",  SyntaxKind.LimitKeyword},

            // INSERT
            { "INTO",  SyntaxKind.InsertKeyword},

            // UPDATE
            { "SET",  SyntaxKind.SetKeyword},
            #endregion

            #region Join
            { "INNER",  SyntaxKind.InnerJoinKeyword},
            { "OUTER",  SyntaxKind.OuterKeyword},
            { "LEFT",  SyntaxKind.LeftJoinKeyword},
            { "RIGHT",  SyntaxKind.RightJoinKeyword},
            { "CROSS",  SyntaxKind.CrossJoinKeyword},
            { "JOIN",  SyntaxKind.JoinKeyword},
            { "ON",  SyntaxKind.OnKeyword},
            #endregion

            #region Conditional Operators
            { "IN",  SyntaxKind.InKeyword},
            { "NOT",  SyntaxKind.NotKeyword},
            { "IS",  SyntaxKind.IsKeyword},
            { "NULL",  SyntaxKind.NullKeyword},
            { "AND",  SyntaxKind.AndKeyword},
            { "OR",  SyntaxKind.OrKeyword},

            #endregion

            #region CASE

            // CASE Specific
            { "CASE",  SyntaxKind.CaseKeyword},
            { "WHEN",  SyntaxKind.WhenKeyword},
            { "THEN",  SyntaxKind.ThenKeyword},
            { "ELSE",  SyntaxKind.ElseKeyword},
            { "END",  SyntaxKind.EndKeyword},

            #endregion

            // Other
            { "AS",  SyntaxKind.AsKeyword},
            { "ALL",  SyntaxKind.AllKeyword},
            { "DISTINCT",  SyntaxKind.DistinctKeyword},
            { "UNIQUE",  SyntaxKind.UniqueKeyword},
            { "ASC",  SyntaxKind.ASCKeyword},
            { "DESC",  SyntaxKind.DESCKeyword},

            // True/False
            { "BOOLEAN",  SyntaxKind.BooleanToken},
            { "TRUE",  SyntaxKind.BooleanToken},
            { "FALSE",  SyntaxKind.BooleanToken},

            // First/Last
            { "FIRST",  SyntaxKind.FirstKeyword},
            { "LAST",  SyntaxKind.LastKeyword},

            #region Functions
            { "AVERAGE",  SyntaxKind.AverageKeyword},
            { "AVG",  SyntaxKind.AverageKeyword},
            { "COUNT",  SyntaxKind.CountKeyword},
            { "MAX",  SyntaxKind.MaxKeyword},
            { "SUM",  SyntaxKind.SumKeyword},

            { "COALESCE",  SyntaxKind.CoalesceKeyword},
            { "POSITION",  SyntaxKind.PositionKeyword},
            { "PARTITION",  SyntaxKind.PartitionKeyword},
            { "RANK",  SyntaxKind.RankKeyword},
            { "ROW_NUMBER",  SyntaxKind.RowNumberKeyword},
            { "SUBSTRING",  SyntaxKind.SubStringKeyword},
            { "SUBSTR",  SyntaxKind.SubStringKeyword},
            { "TRIM",  SyntaxKind.TrimKeyword},
            { "LEADING",  SyntaxKind.LeadingKeyword},
            { "TRAILING",  SyntaxKind.TrailingKeyword},

            { "LOWER",  SyntaxKind.LowerKeyword},
            { "UPPER",  SyntaxKind.UpperKeyword},

            // DATE
            { "DATE_PART",  SyntaxKind.DatePartKeyword},
            { "DATE_TRUNC",  SyntaxKind.DateTruncKeyword},
            { "TIMEOFDAY",  SyntaxKind.TimeOfDayKeyword},
            { "TIMESTAMP",  SyntaxKind.DateTruncKeyword},

            // Conversion
            { "CAST",  SyntaxKind.CastKeyword},
            { "TO_CHAR",  SyntaxKind.ToCharKeyword},
            { "TO_DATE",  SyntaxKind.ToDateKeyword},


            #endregion  
        };
        #endregion

        #region Other Syntax Conversions
        // This dictionary is for other things we may want to convert into
        // a string.. for debugging and error reporting purposes
        private static Dictionary<SyntaxKind, String> SupplementaryConversions = new Dictionary<SyntaxKind, String>()
        {
            // Token Specific
            { SyntaxKind.IdentifierToken            , "Identifier Token"},
            { SyntaxKind.IdentifierColumnSymbol     , "Column Identifier"},
            { SyntaxKind.IdentifierTableSymbol      , "Table Identifier"},
            { SyntaxKind.IdentifierSubQuerySymbol   , "SubQuery Identifier"},
            { SyntaxKind.IdentifierSchemaSymbol     , "Schema Identifier"},
            { SyntaxKind.IdentifierDatabaseSymbol   , "Database Identifier"},
            
            { SyntaxKind.BooleanToken, "Boolean Token"},
            { SyntaxKind.NumericToken, "Numeric Token"},
            { SyntaxKind.LiteralToken, "Literal Token" },
            { SyntaxKind.DateToken, "Date Token"},

            // Trivia
            { SyntaxKind.EOFNode, "End of File" },

            { SyntaxKind.UnknownNode, "Unknown" },
            { SyntaxKind.MissingNode, "Missing" },
        };

        #endregion

        public static Boolean ContainsKey(String xsGiven)
        {
            return Conversions.ContainsKey(xsGiven);
        }

        public static SyntaxKind GetKindFromString (String xsGiven)
        {
            // Perform any cleaning to the string here
            String xsCleanString = xsGiven.ToUpper().Trim();

            // If we have the correct conversion, return it. else unknown
            return Conversions.ContainsKey(xsCleanString) ? Conversions[xsCleanString] : SyntaxKind.UnknownNode;
        }


        public static String GetStringFromKind(SyntaxKind xeKind)
        {
            // This will always return something since the argument is bounded
            // by the Enum
            String xsReturnString = Conversions.FirstOrDefault(x => x.Value == xeKind).Key;

            // Some of the enums do not have a conversion
            if (xsReturnString == null)
            {
                // Check the supplementary conversions dictionary
                if (SupplementaryConversions.ContainsKey(xeKind))
                {
                    xsReturnString = SupplementaryConversions[xeKind];
                }

                
                // Error out since we shouldnt be able to do this
                Debug.Assert(xsReturnString != null);
            }

            //
            return xsReturnString;
        }

    }
}
