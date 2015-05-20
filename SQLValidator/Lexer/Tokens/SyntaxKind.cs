using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLValidator.Lexer.Tokens
{
    public enum SyntaxKind
    {
        // Top Level Statements
        CreateKeyword,
        DeleteKeyword,
        InsertKeyword,

        SelectKeyword,
        UpdateKeyword,
        WithKeyword,
        AlterKeyword,

        // CREATE Specific
        TableKeyword,
        ViewKeyword,
        DistributeOnDeclaration,
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

        // Conditional Keywords/Operators
        InExpression,
        NotExpression,

        // Expressions (All of which will be boolean)
        EqualsExpression,
        NotEqualsExpression,
        GreatherThanExpression,
        GreatherThanOrEqualExpression,
        LessThanExpression,
        LessThanOrEqualExpression,
        AsExpression,
        //HiddenAsExpression,
        

        // Set
        SetExpression,

        // Core
        ColumnIdentifierToken, // Might not need to have this so granular
        TableIdentifierToken,
        AliasIdentifierToken,

        NumericToken,
        VarCharToken,

        // File Specific
        EOFToken,


    }
}