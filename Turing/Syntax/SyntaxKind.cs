using System;

namespace Turing.Syntax
{
    public enum SyntaxKind
    {
        // SAS Specific
        ExecuteKeyword,

        #region Statements
        // Top Level Statements
        CreateKeyword,
        DeleteKeyword,
        InsertKeyword,

        SelectKeyword,
        UpdateKeyword,
        WithKeyword,
        AlterKeyword,
        #endregion

        #region Attributes
        // CREATE Specific
        TableKeyword,
        ViewKeyword,
        DistributeKeyword,
        TempKeyword,
        //ColumnDeclaration, This is an Identifier

        // SELECT Specific
        //ColumnSelectDeclaration, This is an Identifier
        FromKeyword,
        OrderByKeyword,
        GroupByKeyword,
        LimitKeyword,
        WhereKeyword,


        // INSERT
        ValuesKeyword,
        IntoKeyword,

        // JOIN
        InnerJoinKeyword,
        OuterJoinKeyword,
        LeftJoinKeyword,
        RightJoinKeyword,
        CrossJoinKeyword,
        JoinKeyword,
        OnKeyword,

        // UPDATE
        SetKeyword,

        AsKeyword, // Table AS x

        #endregion

        // CASE WHEN
        CaseKeyword,
        WhenKeyword,
        ThenKeyword,
        ElseKeyword,
        EndKeyword,

        // Core Tokens/Types
        IdentifierToken,
        NumericToken,
        LiteralToken,
        BooleanToken,
        
        // Compound Token
        ExpressionNode,

        UnknownToken, // Represents an unknown
        NullNode, // This represents a dead token (not a null keyword)
        EOFToken, // Makes life easier to have this as a token

        // Punctuation
        CommaToken,
        SemiColonToken, // End of Query
        OpenParenthesisToken,
        CloseParenthesisToken,
        DotToken,

        // Basic Operators
        PlusToken,
        MinusToken,
        StarToken, // This could be an all column style identifier
        SlashToken,
        EqualsToken,

        // Conditional Keywords/Operators
        InKeyword,
        NotKeyword,
        IsKeyword, // Eg. IS NOT NULL
        NullKeyword,
        AndKeyword,
        OrKeyword,

        // Compound
        HypenHypenToken, //   --
        SlashStarToken, //    /*
        StarSlashToken, //    */
        DotDotToken, // ..

        DiamondToken, // <>
        EqualsEqualsToken, // ==
        LessThanToken, // <
        LessThanOrEqualToToken, // <=
        GreaterThanToken, // >
        GreaterThanOrEqualToken, // >=

        // FUNCTIONS

        #region Trivia
        // Trivia
        WhitespaceTrivia,
        EmptyCharTrivia,
        EOFTrivia,
        EndOfLineTrivia,
        SingleLineCommentTrivia,
        MultiLineCommentTrivia,

        #endregion
    }
}