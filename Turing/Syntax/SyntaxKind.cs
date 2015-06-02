using System;

namespace Turing.Syntax
{
    public enum SyntaxKind
    {
        // SAS Specific
        ExecuteKeyword,

        #region Statements
        // Top Level Statements (1000 - 1100)  

        SelectKeyword       = 1000,
        DeleteKeyword       = 1010,
        DropKeyword         = 1020,
        InsertKeyword       = 1030,
        UpdateKeyword       = 1040,
        CreateKeyword       = 1050,
        WithKeyword         = 1060,
        AlterKeyword        = 1070,

        // COMMON
        FromKeyword         = 2000, // 
        WhereKeyword        = 2010, // 

        // SELECT Specific
        OrderByKeyword      = 2100,
        GroupByKeyword      = 2110,
        LimitKeyword        = 2120,
        HavingKeyword       = 2130,

        // Common to Crea Column/Expression
        AsKeyword           = 3000,
        // CREATE Specifi3300
        TempKeyword         = 3010,
        TableKeyword        = 3020,
        ViewKeyword         = 3030,
        DistributeKeyword   = 3040,
        // Common to DIST
        OnKeyword           = 4000,

        // INSERT - 2800 
        ValuesKeyword       = 5000,
        IntoKeyword         = 5010,

        // UPDATE - CUSTOM
        SetKeyword          = 6000,

        // ALTER - 3400 -
        AddKeyword          = 7000,
        ColumnKeyword       = 7010,

        // JOIN - NONE
        InnerJoinKeyword    = 8000,
        OuterKeyword        = 8010,
        LeftJoinKeyword     = 8020,
        RightJoinKeyword    = 8030,
        CrossJoinKeyword    = 8040,
        JoinKeyword         = 8050,


        // ORDER BY
        ASCKeyword          = 9000,
        DESCKeyword         = 9010,
        NullsKeyword        = 9020,
        FirstKeyword        = 9021,
        LastKeyword         = 9022,

        // 
        LeadingKeyword      = 9030,
        TrailingKeyword     = 9031,
        BothKeyword         = 9032,

        PartitionKeyword    = 9040,

        //
        AllKeyword          = 10000,
        DistinctKeyword     = 10010,
        UniqueKeyword       = 10020,

        NewKeyword          = 10030,

        #endregion

        #region Expressions

        // Integer Expressions
        AverageKeyword      = 20000, // AVG ()
        CastKeyword         = 20010, // CAST(X AS Y)
        CountKeyword        = 20020,
        MaxKeyword          = 20030, // MAX (SOMETHING)
        SumKeyword          = 20040, // SUM (X)
        PositionKeyword     = 20050,
        RankKeyword         = 20060,
        RowNumberKeyword    = 20070,
        ConcatKeyword       = 20080,

        // Date Expressions
        ExtractKeyword      = 20100, // EXTRACT(day from current_date)
        AddMonthsKeyword    = 20110, // ADD_MONTHS (CURRENT_DATE, 1)
        DatePartKeyword     = 20120, // DATE_PART ('text', timestamp), DATE_PART('text', interval)
        DateTruncKeyword    = 20130, // DATE_TRUNC(text, timestamp)
        TimeOfDayKeyword    = 20140, // TIMEOFDAY()
        TimeStampKeyword    = 20150, // TIMESTAMP (date, time), TIMESTAMP (date)        
        ToDateKeyword       = 20160, // TO_DATE(text, text)

        // Character Expressions
        ToCharKeyword       = 20200, // TO_CHAR(timestamp, text), TO_CHAR(int, text), TO_CHAR (double, text), TO_CHAR(numeric, text)
        SubStringKeyword    = 20210, // SUBSTRING (X, int, int)
        TrimKeyword         = 20220, // TRIM ([leading | trailing | both] texts)
        CoalesceKeyword     = 20230, //
        LowerKeyword        = 20240, // LOWER (TEXT)
        UpperKeyword        = 20250, // UPPER (TEXT)

        #endregion

        #region CASE
        // CASE WHEN
        CaseKeyword         = 22000,
        WhenKeyword         = 22010,
        ThenKeyword         = 22020,
        ElseKeyword         = 22030,
        EndKeyword          = 22040,
        #endregion

        #region Types

        // Core Tokens/Types
        IdentifierToken     = 23000,
        NumericToken        = 23010,
        LiteralToken        = 23020,
        BooleanToken        = 23030,
        DateToken           = 23040,

        #endregion

        #region Punctuation

        // Punctuation 700 - 800
        // Openning Punc
        OpenParenthesisToken    = 11000,

        // Adjunct
        DotToken                = 11010,
        DotDotToken             = 11020, // ..
        CommaToken              = 11030,

        // Comments
        HypenHypenToken         = 11100, //   --
        SlashStarToken          = 11110, //    /*
        StarSlashToken          = 11111, //    */

        // Closing Punc
        SemiColonToken          = 12000, // End of Query
        CloseParenthesisToken   = 12010,

        #endregion

        #region Operators
        // Basic Operators
        PlusToken               = 13000,
        MinusToken              = 13010,
        StarToken               = 13020, // This could be an all column style identifier
        SlashToken              = 13030,

        // Concatinate
        BarBarToken             = 13040, // ||

        // Conditional Keywords/Operators
        EqualsToken             = 14000,
        DiamondToken            = 14010, // <>
        EqualsEqualsToken       = 14020, // ==
        LessThanToken           = 14030, // <
        LessThanOrEqualToToken  = 14040, // <=
        GreaterThanToken        = 14050, // >
        GreaterThanOrEqualToken = 14060, // >=
        InKeyword               = 14070,
        IsKeyword               = 14480, // Eg. IS NOT NULL
        LikeKeyword             = 14490,
        NotKeyword              = 14500,
        NotNullKeyword          = 14510,
        NullKeyword             = 14520,

        // Adjunct
        AndKeyword              = 15000,
        OrKeyword               = 15100,


        #endregion

        #region Trivia
        // Trivia
        WhitespaceTrivia        = 16000,
        EmptyCharTrivia         = 16010,
        EOFTrivia               = 16020,
        EndOfLineTrivia         = 16030,
        SingleLineCommentTrivia = 16040,
        MultiLineCommentTrivia  = 16050,

        #endregion

        // //////////////////////////////////////////////////////////////////////////////////
        // Constructs
        // //////////////////////////////////////////////////////////////////////////////////

        // Column List
        ColumnListNode = 88000, // Necessary to know what it is

        // More specific identifiers
        IdentifierDatabaseSymbol    = 89000,
        IdentifierSchemaSymbol      = 89005,
        IdentifierTableSymbol       = 89010,
        IdentifierSubQuerySymbol    = 89011,
        IdentifierColumnSymbol      = 89015,

        // Custom Nodes
        UnknownNode             = 90000, // Represents an unknown      
        MissingNode             = 90010, // A node that is missing
        NullNode                = 90020, // This represents a dead token (not a null keyword)
        EOFNode                 = 90030, // Makes life easier to have this as a Kind
    }

    
}