﻿namespace CMSSolutions.ContentManagement.Widgets.Scripting.Compiler
{
    public enum TokenKind
    {
        Invalid,
        Eof,
        OpenParen,
        CloseParen,
        StringLiteral,
        SingleQuotedStringLiteral,
        Identifier,
        Integer,
        Comma,
        Plus,
        Minus,
        Mul,
        Div,
        True,
        False,
        And,
        AndSign,
        Or,
        OrSign,
        Not,
        NotSign,
        Equal,
        EqualEqual,
        NotEqual,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        NullLiteral
    }
}